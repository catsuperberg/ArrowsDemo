using System;
using System.Numerics;
using ExtensionMethods;

namespace Game.Gameplay.Meta.Shop
{
    public class ArrowLevelPrice : IItemPriceCalculator
    {
        BigInteger _levelOnePrice = 200;
        BigInteger _growthSpeedCoefficient = 120;
        
        public BigInteger GetPrice(PricingContext context)
        {
            var itemLevel = context.ItemLevel;
            var initialPrice = _levelOnePrice;
            var power = 1.1 + (0.013 * Math.Pow(itemLevel, 1.2));
            var price = initialPrice.PowFractional(power);
            return price;
        }
    }
}