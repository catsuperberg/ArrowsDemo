using System;

namespace Game.GameDesign
{
    public class AveragePlayerData
    {
        public readonly float AverageWorseGateChance;
        public readonly float AdMultiplier;
        public readonly WeightedBuyerTypeProvider BuyerTypeProvider;
        public readonly UpgradeAtRunIndexCalculator UpgradeCountCalculator;

        public AveragePlayerData(float averageWorseGateChance, float adMultiplier, WeightedBuyerTypeProvider buyerTypeProvider, UpgradeAtRunIndexCalculator upgradeCountCalculator)
        {
            AverageWorseGateChance = averageWorseGateChance;
            AdMultiplier = adMultiplier;
            BuyerTypeProvider = buyerTypeProvider ?? throw new ArgumentNullException(nameof(buyerTypeProvider));
            UpgradeCountCalculator = upgradeCountCalculator ?? throw new ArgumentNullException(nameof(upgradeCountCalculator));
        }
    }
    
    public class PlaythroughSimulatorFactory
    {
        UpgradeBuyerFactory _buyerFactory;
        RunSimulator.Factory _runSimFactory;

        public PlaythroughSimulatorFactory(UpgradeBuyerFactory buyerFactory, RunSimulator.Factory runSimFactory)
        {
            _buyerFactory = buyerFactory ?? throw new ArgumentNullException(nameof(buyerFactory));
            _runSimFactory = runSimFactory ?? throw new ArgumentNullException(nameof(runSimFactory));
        }

        public PlaythroughSimulator CreateRandom()
        {            
            var rand = new System.Random(this.GetHashCode() + System.DateTime.Now.GetHashCode());
            var gateSelector = new GateSelector(GateSelectorGrades.GetRandomGradeChance(rand));
            var adSelector = AdSelectorGrades.GetRandomGrade(rand);
            
            var upgradeBuyer = _buyerFactory.GetBuyer(_buyerFactory.GetRandomGrade(rand)); 
            var actors = new PlayerActors(gateSelector, adSelector, upgradeBuyer);
            var player = new VirtualPlayer(actors);
            return new PlaythroughSimulator(player, _runSimFactory.Create());
        }
        
        public PlaythroughSimulator CreateAverage(AveragePlayerData playerData)
        {            
            var gateSelector = new GateSelector(playerData.AverageWorseGateChance, false);
            var adSelector = new StaticAdSelector(playerData.AdMultiplier);
            var upgradeCountBuyer = _buyerFactory.GetCountBuyer(playerData.BuyerTypeProvider);
            
            var upgradeBuyer = new UnimplimentedBuyer(); 
            var actors = new PlayerActors(gateSelector, adSelector, upgradeBuyer);
            var player = new AverageVirtualPlayer(actors, upgradeCountBuyer, playerData.UpgradeCountCalculator);
            return new PlaythroughSimulator(player, _runSimFactory.Create());
        }
    }
}