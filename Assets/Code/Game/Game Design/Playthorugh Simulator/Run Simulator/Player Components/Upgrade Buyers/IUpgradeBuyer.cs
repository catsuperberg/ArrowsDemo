using Game.Gameplay.Meta.UpgradeSystem;
using System.Numerics;

namespace Game.GameDesign
{
    public interface IUpgradeBuyer
    {        
        UpgradeResults BuyAll(UpgradeContext originalContext, BigInteger PointsToSpend);
    }
}