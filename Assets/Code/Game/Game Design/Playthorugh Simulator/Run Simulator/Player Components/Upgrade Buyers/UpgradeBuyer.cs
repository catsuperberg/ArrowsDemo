using Game.Gameplay.Meta.UpgradeSystem;
using GameMath;
using System;
using System.Collections.Generic;
using System.Numerics;
using Utils;
using ExtensionMethods;

namespace Game.GameDesign
{
    public class UpgradeBuyerFactory
    {
        SimpleUpgradePricing _pricing;
        
        readonly Dictionary<int, Action<UpgradeContainer[], FastRandom>> _gradeFrequencies;     

        public UpgradeBuyerFactory(SimpleUpgradePricing pricing)
        {
            _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
            _gradeFrequencies = new Dictionary<int, Action<UpgradeContainer[], FastRandom>>(){
                    {4, SortTypes.SortHighToLow},
                    {3, SortTypes.SortLowToHigh},
                    {2, SortTypes.SortRandom}};
        }

        public Action<UpgradeContainer[], FastRandom> GetRandomGrade(Random rand)
            => WeightedRandom.NextFrom(_gradeFrequencies, rand);
            
        public IUpgradeBuyer GetBuyer(Action<UpgradeContainer[], FastRandom> sortType)
            => new SortedBuyer(_pricing, sortType);
    }
    
    public class SortedBuyer : Buyer, IUpgradeBuyer
    {
        private readonly SimpleUpgradePricing _pricing;
        Action<UpgradeContainer[], FastRandom> _sort;
        const float _randomBuyerChance = 0.25f;
        FastRandom _randFast = new FastRandom(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());

        public SortedBuyer(SimpleUpgradePricing pricing, Action<UpgradeContainer[], FastRandom> sort)
        {
            _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
            _sort = sort;
        } 
        
        public UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend)
        {
            var chanceCheck = _randFast.NextDouble();
            var useRandomInstead = chanceCheck < _randomBuyerChance;
            var sort = useRandomInstead ? SortTypes.Random(_randFast) : _sort;
            
            var pointsLeft = PointsToSpend;
            var upgrades = ContextToUpgrades(originalContext, _pricing);
            var count = 0;            
            while (true)
            {
                int upgradeToBuy = -1;
                sort(upgrades, _randFast);
                for(int i = 0; i < upgrades.Length; i++)
                    if(upgrades[i].Price <= pointsLeft)
                        {upgradeToBuy = i; break;}   
                
                if(upgradeToBuy == -1) break;
                pointsLeft -= upgrades[upgradeToBuy].Price;
                upgrades[upgradeToBuy].IncrementLevel();
                count++;
            }            
            return new UpgradeResults(UpgradesToContext(upgrades), count, pointsLeft);
        }
    }
    
    public static class SortTypes
    {  
        public static Action<UpgradeContainer[], FastRandom> Random(FastRandom rand)
        {
            var index = rand.Next(3);
            switch(index)
            {
                case 0: return SortHighToLow;
                case 1: return SortLowToHigh;
                case 2: return SortRandom;
                default: throw new Exception($"No sort type like this: {index}");
            }
        }
        
        public static void SortHighToLow(UpgradeContainer[] upgrades, FastRandom rand)
            => Array.Sort<UpgradeContainer>(upgrades, new Comparison<UpgradeContainer>((i1, i2) => i2.CompareTo(i1)));
                    
        public static void SortLowToHigh(UpgradeContainer[] upgrades, FastRandom rand)
            => Array.Sort(upgrades);   
            
        public static void SortRandom(UpgradeContainer[] upgrades, FastRandom rand)
            => upgrades.Shuffle(rand);  
    }
}