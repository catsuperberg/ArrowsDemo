using Game.Gameplay.Meta.PassiveIncome;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Meta.Curencies;

namespace Game.Gameplay.Meta
{
    public class UserContext
    {
        public readonly CurenciesContext Curencies; 
        public readonly UpgradeContext Upgrades;  
        public readonly PassiveInvomceContext PassiveInvomce;     
        
        public UserContext(CurenciesContext curencies, UpgradeContext upgrades, PassiveInvomceContext passiveInvomce)
        {
            Curencies = curencies;
            Upgrades = upgrades;
            PassiveInvomce = passiveInvomce;
        }
    }
}