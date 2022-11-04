using System;

namespace Game.GameDesign
{
    public class PlaythroughSimulatorFactory
    {
        PlaythroughEndConditions _endConditions;
        UpgradeBuyerFactory _buyerFactory;
        RunSimulator.Factory _runSimFactory;

        public PlaythroughSimulatorFactory(UpgradeBuyerFactory buyerFactory, RunSimulator.Factory runSimFactory, PlaythroughEndConditions endConditions)
        {
            _buyerFactory = buyerFactory ?? throw new ArgumentNullException(nameof(buyerFactory));
            _runSimFactory = runSimFactory ?? throw new ArgumentNullException(nameof(runSimFactory));
            _endConditions = endConditions ?? throw new ArgumentNullException(nameof(endConditions));
        }

        public PlaythroughSimulator CreateRandom()
        {            
            var rand = new System.Random(this.GetHashCode() + System.DateTime.Now.GetHashCode());
            var gateSelector = new GateSelector(GateSelectorGrades.GetRandomGradeChance(rand));
            var adSelector = AdSelectorGrades.GetRandomGrade(rand);
            
            var upgradeBuyer = _buyerFactory.GetRandomGrade(rand);         
            var actors = new PlayerActors(gateSelector, adSelector, upgradeBuyer);
            var player = new VirtualPlayer(actors);
            return new PlaythroughSimulator(player, _runSimFactory.Create(), _endConditions);
        }
    }
}