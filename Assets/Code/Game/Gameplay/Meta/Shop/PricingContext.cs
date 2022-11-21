using System.Numerics;

namespace Game.Gameplay.Meta.Shop
{
    public struct PricingContext
    {
        public readonly int ItemLevel;

        public PricingContext(int itemLevel)
        {
            ItemLevel = itemLevel;
        }
    }
}