using System.Numerics;

namespace Game.Gameplay.Meta.Shop
{
    public interface IItemPriceCalculator
    {
        BigInteger GetPrice(PricingContext context);
        BigInteger GetPrice(int currentLevel);
    }
}