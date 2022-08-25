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
        public readonly SkinCollection ProjectileSkins;    
        public readonly SkinCollection CrossbowSkins;

        public UserContext(CurenciesContext curencies, UpgradeContext upgrades, PassiveInvomceContext passiveInvomce, SkinCollection projectileSkins, SkinCollection crossbowSkins)
        {
            Curencies = curencies;
            Upgrades = upgrades;
            PassiveInvomce = passiveInvomce;
            ProjectileSkins = projectileSkins;
            CrossbowSkins = crossbowSkins;
        }
    }
}