using System.Numerics;

namespace Game.Gameplay.Meta.Shop
{
    public class PricingContext
    {
        public readonly int ItemLevel;

        public PricingContext(int itemLevel)
        {
            ItemLevel = itemLevel;
        }
    }
}