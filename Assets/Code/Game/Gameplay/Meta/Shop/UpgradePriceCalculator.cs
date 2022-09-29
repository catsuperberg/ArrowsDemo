using System;
using System.Numerics;

namespace Game.Gameplay.Meta.Shop
{
    public class UpgradePriceCalculator : IItemPriceCalculator
    {
        UpgradePriceFormula _formula;
        
        public UpgradePriceCalculator(UpgradePriceFormula formula)
        {
            _formula = formula ?? throw new ArgumentNullException(nameof(formula));
        }
        
        public BigInteger GetPrice(PricingContext context)
        {
            return _formula.Evaluate(context.ItemLevel);
        }
    }
}