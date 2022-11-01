
using Game.Gameplay.Meta.UpgradeSystem;
using System.Collections.Generic;
using System.Linq;

namespace Game.GameDesign
{
    public class Buyer
    {        
        protected static List<UpgradeContainer> ContextToUpgrades(UpgradeContext context, SimpleUpgradePricing pricing)
        {            
            var upgrades = new List<UpgradeContainer>();
            upgrades.Add(new UpgradeContainer(nameof(context.CrossbowLevel), context.CrossbowLevel, pricing.UpgradePrice));
            upgrades.Add(new UpgradeContainer(nameof(context.ArrowLevel), context.ArrowLevel, pricing.UpgradePrice));
            upgrades.Add(new UpgradeContainer(nameof(context.InitialArrowCount), context.InitialArrowCount, pricing.UpgradePrice));
            return upgrades;
        }
        
        protected static UpgradeContext UpgradesToContext(List<UpgradeContainer> upgrades)
        {
            var crossbowLevel = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.CrossbowLevel)).Level;
            var arrowLevel = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.ArrowLevel)).Level;
            var initialArrowCount = upgrades.FirstOrDefault(entry => entry.Name is nameof(UpgradeContext.InitialArrowCount)).Level;
            return new UpgradeContext(crossbowLevel, arrowLevel, initialArrowCount);
        }
    }
}