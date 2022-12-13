using System;
using Utils;
using ExtensionMethods;

namespace Game.GameDesign
{
    public static class Sort
    {                  
        public static Action<UpgradeContainer[], FastRandom> ByType(BuyerType type)
        {
            switch(type)
            {
                case BuyerType.HighestPriceFirst: return SortHighToLow;
                case BuyerType.LowestPriceFirst: return SortLowToHigh;
                case BuyerType.RandomAffordable: return SortRandom;
                default: throw new Exception($"No sort type like this: {type}");
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