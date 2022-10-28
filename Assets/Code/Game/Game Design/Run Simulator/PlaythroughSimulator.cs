using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.GameDesign
{
    public class PlaythroughSimulator
    {
        const int _endPlaythroughGateCount = 50;    
    
        readonly VirtualPlayer _player;
        readonly RunSimulator _runSimulator;
        readonly SimulationSequnceContextProvider _contextProvider;

        public PlaythroughSimulator(VirtualPlayer player, RunSimulator runSimulator)
        {
            _player = player ?? throw new System.ArgumentNullException(nameof(player));
            _runSimulator = runSimulator ?? throw new System.ArgumentNullException(nameof(runSimulator));
            _contextProvider = new SimulationSequnceContextProvider(_player.Context);
        }
        
        
        public PlaythroughData Simulate()
        {
            var results = new List<RunData>();
            SequenceContext sequenceContext = null;
            do
            {
                sequenceContext = _player.Context.SequenceContext;
                var result = _runSimulator.Simulate(sequenceContext);
                _player.RecieveReward(result.FinalScore);                
                _player.BuyUpgrades();
                results.Add(result);
            }while(sequenceContext.NumberOfOperations <= _endPlaythroughGateCount);
            
            return new PlaythroughData(results);
        }
    }
}