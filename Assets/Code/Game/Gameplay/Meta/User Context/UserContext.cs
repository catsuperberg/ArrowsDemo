using Game.Gameplay.Meta.PassiveIncome;
using Game.Gameplay.Meta.UpgradeSystem;

namespace Game.Gameplay.Meta
{
    public class UserContext
    {
        public readonly UpgradeContext Upgrades;  
        public readonly PassiveInvomceContext PassiveInvomce;     
        
        public UserContext(UpgradeContext upgrades, PassiveInvomceContext passiveInvomce)
        {
            Upgrades = upgrades;
            PassiveInvomce = passiveInvomce;
        }
    }
}