using System;

namespace Game.GameDesign
{
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
    }
}