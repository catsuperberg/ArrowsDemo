using Game.Gameplay.Meta.UpgradeSystem;
using System.Numerics;

namespace Game.GameDesign
{
    public struct UpgradeResults
    {
        public readonly UpgradeContext NewUpgrades;
        public readonly int UpgradesBought;
        public readonly BigInteger PointsLeft;
        public readonly BuyerType TypeUsed;

        public UpgradeResults(UpgradeContext newUpgrades, int upgradesBought, BigInteger pointsLeft, BuyerType typeUsed)
        {
            NewUpgrades = newUpgrades;
            UpgradesBought = upgradesBought;
            PointsLeft = pointsLeft;
            TypeUsed = typeUsed;
        }
    }
}