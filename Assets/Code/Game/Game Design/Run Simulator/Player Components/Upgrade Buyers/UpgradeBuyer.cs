using Game.Gameplay.Meta.UpgradeSystem;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public class UpgradeBuyerFactory
    {
        SimpleUpgradePricing _pricing;
        
        readonly Dictionary<int, IUpgradeBuyer> _gradeFrequencies;        
        readonly IReadOnlyDictionary<Type, IUpgradeBuyer> _buyers;

        public UpgradeBuyerFactory(SimpleUpgradePricing pricing)
        {
            _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
            _gradeFrequencies = new Dictionary<int, IUpgradeBuyer>(){
                    {3, new HighestPriceBuyer(_pricing)},
                    {5, new LowestPriceBuyer(_pricing)},
                    {2, new RandomBuyer(_pricing)}};
            _buyers = new Dictionary<Type, IUpgradeBuyer>(){
                    {typeof(HighestPriceBuyer), new HighestPriceBuyer(_pricing)},
                    {typeof(LowestPriceBuyer), new LowestPriceBuyer(_pricing)},
                    {typeof(RandomBuyer), new RandomBuyer(_pricing)}};
        }

        public IUpgradeBuyer GetRandomGrade()
            => WeightedRandom.NextFrom(_gradeFrequencies);
            
        public IUpgradeBuyer GetBuyer(Type type)
            => _buyers[type];
    }

    public class HighestPriceBuyer : Buyer, IUpgradeBuyer 
    {
        IUpgradeBuyer _buyer;
                
        public HighestPriceBuyer(SimpleUpgradePricing pricing)
        {
            _buyer = new SortedBuyer(pricing, HighestToLowest: true);
        } 
        
        public UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend)
            => _buyer.BuyAll(originalContext, PointsToSpend); 
    }
    
    public class LowestPriceBuyer : Buyer, IUpgradeBuyer 
    {      
        IUpgradeBuyer _buyer;
                
        public LowestPriceBuyer(SimpleUpgradePricing pricing)
        {
            _buyer = new SortedBuyer(pricing, HighestToLowest: false);
        } 
        
        public UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend)
            => _buyer.BuyAll(originalContext, PointsToSpend); 
    }
    
    public class RandomBuyer : Buyer, IUpgradeBuyer
    {     
        private readonly SimpleUpgradePricing _pricing;
        Random _rand;

        public RandomBuyer(SimpleUpgradePricing pricing)
        {
            _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
            _rand = new Random(this.GetHashCode());
        } 
        
        public UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend)
        {
            var pointsLeft = PointsToSpend;
            var upgrades = ContextToUpgrades(originalContext, _pricing);
            var numVariants = upgrades.Count;
            var count = 0;            
            while (upgrades.Any(entry => entry.Price <= pointsLeft))
            {
                var randomChoice = _rand.Next(numVariants);
                var upgradeToBuy = upgrades.ElementAt(randomChoice);
                if(upgradeToBuy.Price <= pointsLeft)
                {
                    pointsLeft -= upgradeToBuy.Price;
                    upgradeToBuy.Level++;
                    count++;
                }
            }            
            return new UpgradeResults(UpgradesToContext(upgrades), count, pointsLeft);
        }
    }
    
    public class SortedBuyer : Buyer, IUpgradeBuyer
    {
        private readonly SimpleUpgradePricing _pricing;
        Func<IEnumerable<UpgradeContainer>, IEnumerable<UpgradeContainer>> _sort;

        public SortedBuyer(SimpleUpgradePricing pricing, bool HighestToLowest)
        {
            _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
            _sort = HighestToLowest ? SortHighToLow : SortLowToHigh;
        } 
        
        public UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend)
        {
            var pointsLeft = PointsToSpend;
            var upgrades = ContextToUpgrades(originalContext, _pricing);
            var count = 0;            
            while (true)
            {
                var sortedByPrice = _sort(upgrades);
                var upgradeToBuy = sortedByPrice.FirstOrDefault(entry => entry.Price <= pointsLeft);
                if(upgradeToBuy == null) break;
                pointsLeft -= upgradeToBuy.Price;
                upgradeToBuy.Level++;
                count++;
            }            
            return new UpgradeResults(UpgradesToContext(upgrades), count, pointsLeft);
        }
        
        IEnumerable<UpgradeContainer> SortHighToLow(IEnumerable<UpgradeContainer> upgrades)
            => from upgrade in upgrades 
                    orderby upgrade.Price descending
                    select upgrade;
                    
        IEnumerable<UpgradeContainer> SortLowToHigh(IEnumerable<UpgradeContainer> upgrades)
            => from upgrade in upgrades 
                    orderby upgrade.Price ascending
                    select upgrade;        
    }
}