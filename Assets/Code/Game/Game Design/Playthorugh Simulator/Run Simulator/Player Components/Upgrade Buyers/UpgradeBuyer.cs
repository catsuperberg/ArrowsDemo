using Game.Gameplay.Meta.UpgradeSystem;
using GameMath;
using System;
using System.Collections.Generic;
using System.Numerics;
using Utils;

namespace Game.GameDesign
{
    public class UpgradeBuyerFactory
    {        
        SimpleUpgradePricing _pricing;
        
        readonly Dictionary<int, BuyerType> _gradeFrequencies;     

        public UpgradeBuyerFactory(SimpleUpgradePricing pricing)
        {
            _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
            _gradeFrequencies = new Dictionary<int, BuyerType>(){
                    {4, BuyerType.HighestPriceFirst},
                    {3, BuyerType.LowestPriceFirst},
                    {2, BuyerType.RandomAffordable}};
        }

        public BuyerType GetRandomGrade(Random rand)
            => WeightedRandom.NextFrom(_gradeFrequencies, rand);
            
        public IUpgradeBuyer GetBuyer(BuyerType buyerType)
            => new SortedBuyer(_pricing, buyerType);            
            
        public CountBuyer GetCountBuyer(WeightedBuyerTypeProvider typeProvider)
            => new CountBuyer(_pricing, typeProvider);
    }
    
    public class SortedBuyer : Buyer, IUpgradeBuyer
    {
        SimpleUpgradePricing _pricing;
        BuyerType _assignedType;
        Action<UpgradeContainer[], FastRandom> _assignedSortFunction;
        const float _randomBuyerChance = 0.25f;
        FastRandom _randFast = new FastRandom(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());

        public SortedBuyer(SimpleUpgradePricing pricing, BuyerType type)
        {
            _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
            _assignedType = type;
            _assignedSortFunction = Sort.ByType(_assignedType);
        } 
        
        public UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend)
        {
            var chanceCheck = _randFast.NextDouble();
            var useRandomInstead = chanceCheck < _randomBuyerChance;
            
            var type = useRandomInstead ? RandomType() : _assignedType;
            var sortFunction = useRandomInstead ? Sort.ByType(type) : _assignedSortFunction;
            
            var pointsLeft = PointsToSpend;
            var upgrades = ContextToUpgrades(originalContext, _pricing);
            var count = 0;            
            while (true)
            {
                int upgradeToBuy = -1;
                sortFunction(upgrades, _randFast);
                for(int i = 0; i < upgrades.Length; i++)
                    if(upgrades[i].Price <= pointsLeft)
                        {upgradeToBuy = i; break;}   
                
                if(upgradeToBuy == -1) break;
                pointsLeft -= upgrades[upgradeToBuy].Price;
                upgrades[upgradeToBuy].IncrementLevel();
                count++;
            }            
            return new UpgradeResults(UpgradesToContext(upgrades), count, pointsLeft, type);
        }
        
        BuyerType RandomType()
            => (BuyerType)_randFast.Next((int)BuyerType.Count);
    }
    
    public class CountBuyer : Buyer
    {
        SimpleUpgradePricing _pricing;
        WeightedBuyerTypeProvider _typeProvider;
        
        Random _rand = new Random(Guid.NewGuid().GetHashCode());
        FastRandom _randFast = new FastRandom(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());
        
        public CountBuyer(SimpleUpgradePricing pricing, WeightedBuyerTypeProvider typeProvider)
        {
            _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
            _typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
        }
        
        public UpgradeResults BuyAll(UpgradeContext originalContext, int UpgradesToBuy)
        {
            var type = _typeProvider.NextBuyerType(_rand);
            
            var sortFunction = Sort.ByType(type);
            
            var upgrades = ContextToUpgrades(originalContext, _pricing);
            var count = 0;            
            while (count < UpgradesToBuy)
            {
                int upgradeToBuy = -1;
                sortFunction(upgrades, _randFast);
                for(int i = 0; i < upgrades.Length; i++)
                    {upgradeToBuy = i; break;}   
                
                if(upgradeToBuy == -1) break;
                upgrades[upgradeToBuy].IncrementLevel();
                count++;
            }            
            return new UpgradeResults(UpgradesToContext(upgrades), count, 0, type);
        }
    }
    
    public class UnimplimentedBuyer : Buyer, IUpgradeBuyer
    {        
        public UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend)
            => throw new NotImplementedException();
    }
}