using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{        
    public interface IUpgradeBuyer
    {        
        UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend);
    }
    
    public class UpgradeResults
    {
        public readonly UpgradeContext NewUpgrades;
        public readonly int UpgradesBought;
        public readonly BigInteger PointsLeft;

        public UpgradeResults(UpgradeContext newUpgrades, int upgradesBought, BigInteger pointsLeft)
        {
            NewUpgrades = newUpgrades;
            UpgradesBought = upgradesBought;
            PointsLeft = pointsLeft;
        }
    }
    
    // public static class UpgradeBuyerGrades
    // {
    //     static Dictionary<int, IUpgradeBuyer> _gradeFrequencies = new Dictionary<int, IAdSelector>(){
    //                 {3, new FastAdSelector()},
    //                 {5, new SlowAdSelector()},
    //                 {2, new AdSkipper()}};                
                
    //     public static IUpgradeBuyer GetRandomGrade()
    //         => WeightedRandom.NextFrom(_gradeFrequencies);
    // }
    
    public class SimpleUpgradePricing
    {      
        PriceCalculatorFactory _priceCalculators;
        
        public SimpleUpgradePricing(PriceCalculatorFactory priceCalculators)
        {
            _priceCalculators = priceCalculators ?? throw new ArgumentNullException(nameof(priceCalculators));            
        }
        
        public BigInteger UpgradePrice(string upgradeName, int currentLevel)
            => _priceCalculators.GetCalculatorFor(upgradeName).GetPrice(new PricingContext(currentLevel));
    }
    
    public class UpgradeContainer
    {
        public string Name;
        public int Level;
        public BigInteger Price {get => _getPrice(Name, Level);}
        
        Func<string, int, BigInteger> _getPrice;
        
        public UpgradeContainer(string name, int level, Func<string, int, BigInteger> priceFunction)
        {
            Name = name;
            Level = level;
            _getPrice = priceFunction;
        }
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
    
    public class Buyer
    {        
        protected static List<UpgradeContainer> ContextToUpgrades(UpgradeContext context, SimpleUpgradePricing pricing)
        {            
            var upgrades = new List<UpgradeContainer>();
            upgrades.Add(new UpgradeContainer(nameof(context.CrossbowLevel), context.CrossbowLevel, pricing.UpgradePrice));
            upgrades.Add(new UpgradeContainer(nameof(context.ArrowLevel), context.ArrowLevel, pricing.UpgradePrice));
            upgrades.Add(new UpgradeContainer(nameof(context.InitialArrowCount), context.InitialArrowCount, pricing.UpgradePrice));
            return upgrades;
        }
        
        protected static UpgradeContext UpgradesToContext(List<UpgradeContainer> upgrades)
        {
            var crossbowLevel = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.CrossbowLevel)).Level;
            var arrowLevel = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.ArrowLevel)).Level;
            var initialArrowCount = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.InitialArrowCount)).Level;
            return new UpgradeContext(crossbowLevel, arrowLevel, initialArrowCount);
        }
    }
}