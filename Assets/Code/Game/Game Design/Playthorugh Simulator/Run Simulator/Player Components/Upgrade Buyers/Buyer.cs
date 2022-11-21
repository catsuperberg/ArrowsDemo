using Game.Gameplay.Meta.UpgradeSystem;
// using System.Collections.Generic;
// using System.Linq;

namespace Game.GameDesign
{
    public class Buyer
    {        
        protected static UpgradeContainer[] ContextToUpgrades(UpgradeContext context, SimpleUpgradePricing pricing)
        {            
            var upgrades = new UpgradeContainer[3];
            upgrades[0] = (new UpgradeContainer(nameof(context.CrossbowLevel), context.CrossbowLevel, pricing.UpgradePrice));
            upgrades[1] = (new UpgradeContainer(nameof(context.ArrowLevel), context.ArrowLevel, pricing.UpgradePrice));
            upgrades[2] = (new UpgradeContainer(nameof(context.InitialArrowCount), context.InitialArrowCount, pricing.UpgradePrice));
            return upgrades;
        }
        
        protected static UpgradeContext UpgradesToContext(UpgradeContainer[] upgrades)
        {
            // var crossbowLevel = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.CrossbowLevel)).Level;
            // var arrowLevel = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.ArrowLevel)).Level;
            // var initialArrowCount = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.InitialArrowCount)).Level;
            var crossbowLevel = upgrades[0].Level;
            var arrowLevel = upgrades[1].Level;
            var initialArrowCount = upgrades[2].Level;
            return new UpgradeContext(crossbowLevel, arrowLevel, initialArrowCount);
        }
    }
}