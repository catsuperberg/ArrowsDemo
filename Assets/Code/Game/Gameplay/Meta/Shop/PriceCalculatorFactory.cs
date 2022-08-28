using System;
using System.Linq;
using System.Collections.Generic;
using Game.Gameplay.Meta.UpgradeSystem;

namespace Game.Gameplay.Meta.Shop
{
    public static class PriceCalculatorFactory
    {
        static List<IItemPriceCalculator> _calculatorsCache = new List<IItemPriceCalculator>();
        
        public static IItemPriceCalculator GetCalculatorFor(string variableName)
        {
            IItemPriceCalculator priceCalculator;
            switch (variableName)
            {
                case nameof(UpgradeContext.InitialArrowCount):
                    priceCalculator = new InitialArrowsPrice();
                    break;
                case nameof(UpgradeContext.ArrowLevel):
                    priceCalculator = new ArrowLevelPrice(); 
                    break;
                case nameof(UpgradeContext.CrossbowLevel):
                    priceCalculator = new CrossbowLevelPrice(); 
                    break;
                case nameof(UpgradeContext.PassiveIncome):
                    // priceCalculator = new PassiveIncomePrice(); 
                    // break;
                default:
                    throw new Exception("Item price calculator factory doesn't know what to do with: " + variableName);
            }
            
            _calculatorsCache.Add(priceCalculator);
            
            return priceCalculator;
        }
        
        static IItemPriceCalculator GetFromCacheIfPresent(Type calculatorType)
        {
            var cachedCalculator = _calculatorsCache.First(instance => instance.GetType() == calculatorType);
            return (cachedCalculator != null) ? cachedCalculator : CreateAndAddToCache(calculatorType);
        }
        
        static IItemPriceCalculator CreateAndAddToCache(Type calculatorType)
        {
            var instance = (IItemPriceCalculator)Activator.CreateInstance(calculatorType);
            _calculatorsCache.Add(instance);
            return instance;
        }
    }
}