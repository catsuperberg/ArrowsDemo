using Game.Gameplay.Meta.PassiveIncome;
using Game.Gameplay.Meta.UpgradeSystem;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Meta.Skins;

namespace Game.Gameplay.Meta
{
    public class UserContext
    {
        public readonly CurenciesContext Curencies; 
        public readonly UpgradeContext Upgrades;  
        public readonly PassiveInvomceContext PassiveInvomce;     
        public readonly ProjectileCollection ProjectileSkins;

        public UserContext(CurenciesContext curencies, UpgradeContext upgrades, PassiveInvomceContext passiveInvomce, ProjectileCollection projectileSkins)
        {
            Curencies = curencies;
            Upgrades = upgrades;
            PassiveInvomce = passiveInvomce;
            ProjectileSkins = projectileSkins;
        }
    }
}