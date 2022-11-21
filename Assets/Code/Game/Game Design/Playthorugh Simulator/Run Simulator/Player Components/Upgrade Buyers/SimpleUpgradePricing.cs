using Game.Gameplay.Meta.Shop;
using System;
using System.Numerics;

namespace Game.GameDesign
{
    public class SimpleUpgradePricing
    {      
        PriceCalculatorFactory _priceCalculators;
        
        public SimpleUpgradePricing(PriceCalculatorFactory priceCalculators)
        {
            _priceCalculators = priceCalculators ?? throw new ArgumentNullException(nameof(priceCalculators));            
        }
        
        public BigInteger UpgradePrice(string upgradeName, int currentLevel)
            => _priceCalculators.GetCalculatorFor(upgradeName).GetPrice(currentLevel);
            // => _priceCalculators.GetCalculatorFor(upgradeName).GetPrice(new PricingContext(currentLevel));
    }
}