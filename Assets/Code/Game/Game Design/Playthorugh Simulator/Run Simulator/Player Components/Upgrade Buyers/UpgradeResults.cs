using Game.Gameplay.Meta.UpgradeSystem;
using System.Numerics;

namespace Game.GameDesign
{
    public struct UpgradeResults
    {
        public readonly UpgradeContext NewUpgrades;
        public readonly int UpgradesBought;
        public readonly BigInteger PointsLeft;

        public UpgradeResults(UpgradeContext newUpgrades, int upgradesBought, BigInteger pointsLeft)
        {
            NewUpgrades = newUpgrades;
            UpgradesBought = upgradesBought;
            PointsLeft = pointsLeft;
        }
    }
}