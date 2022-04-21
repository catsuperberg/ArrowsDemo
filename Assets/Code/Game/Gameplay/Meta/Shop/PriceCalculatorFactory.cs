using System;
using Game.Gameplay.Meta.UpgradeSystem;

namespace Game.Gameplay.Meta.Shop
{
    public class PriceCalculatorFactory
    {
        public IItemPriceCalculator GetCalculatorFor(string variableName)
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
                default:
                    throw new Exception("Item price calculator factory doesn't know what to do with: " + variableName);
            }
            
            return priceCalculator;
        }
    }
}