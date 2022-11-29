using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Collections.Generic;
using System.Linq;

namespace Game.GameDesign
{    
    public class PlaythroughSimulator
    {
        readonly VirtualPlayer _player;
        readonly RunSimulator _runSimulator;
        readonly SimulationSequnceContextProvider _contextProvider;

        public PlaythroughSimulator(VirtualPlayer player, RunSimulator runSimulator)
        {
            _player = player ?? throw new System.ArgumentNullException(nameof(player));
            _runSimulator = runSimulator ?? throw new System.ArgumentNullException(nameof(runSimulator));
            _contextProvider = new SimulationSequnceContextProvider(_player.Context);
        }        
        
        public PlaythroughData Simulate(CompletionConditions completionConditions)
        {
            _player.Reset();
            var results = new List<RunData>();
            IEnumerable<EndCondition> endRichedConditions;
            RunData lastResult;
            SequenceContext sequenceContext;
            do
            {
                sequenceContext = _player.Context.SequenceContext;
                lastResult = _runSimulator.Simulate(sequenceContext, _player.Actors);
                _player.RecieveReward(lastResult.FinalScore);                
                _player.BuyUpgrades();
                results.Add(lastResult);
                endRichedConditions = completionConditions.ConditionsThatMet(lastResult, PlaythroughData.CombineTime(results));
            }while(!endRichedConditions.Any());
            
            return new PlaythroughData(results, _player.HeaderString, _player.Context.UpgradesPerIteration, endRichedConditions);
        }
        
        public PlaythroughData[] Simulate(int count, CompletionConditions completionConditions)
        {
            var result = new PlaythroughData[count];
            for(int i = 0; i < result.Length; i++)
                result[i] = Simulate(completionConditions);
            return result;
        }
    }
}