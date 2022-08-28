using System;
using System.Numerics;
using ExtensionMethods;

namespace Game.Gameplay.Meta.Shop
{
    public class CrossbowLevelPrice : IItemPriceCalculator
    {
        BigInteger _levelOnePrice = 520;
        BigInteger _growthSpeedCoefficient = 120;
        
        public BigInteger GetPrice(PricingContext context)
        {
            var itemLevel = context.ItemLevel;
            var initialPrice = _levelOnePrice;
            var power = 1.1 + (0.022 * Math.Pow(itemLevel, 1.2));
            var price = initialPrice.PowFractional(power);
            return price;
        }
    }
}