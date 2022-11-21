using Game.Gameplay.Realtime.OperationSequence.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public enum EndCondition
    {
        Score,
        GameplayTime,
        PlayTime
    }
    
    public struct PlaythroughEndConditions
    {
        TimeSpan _playTime;
        TimeSpan _gameplayTimeOfRun;
        BigInteger _maxReward;

        public PlaythroughEndConditions(TimeSpan playTime, TimeSpan runTime, BigInteger maxReward)
        {
            _playTime = playTime;
            _gameplayTimeOfRun = runTime;
            _maxReward = maxReward;
        }

        public bool MetAny(RunData data, TimeSpan combinedTime)
        {
            var met = ConditionsThatMet(data, combinedTime);
            return met.Any();
        }
        
        public IEnumerable<EndCondition> ConditionsThatMet(RunData data, TimeSpan combinedTime)
        {
            var met = new List<EndCondition>();
            if(data.FinalScore >= _maxReward) met.Add(EndCondition.Score);
            if(data.LevelRunTime >= _gameplayTimeOfRun) met.Add(EndCondition.GameplayTime);
            if(combinedTime >= _playTime) met.Add(EndCondition.PlayTime);
            return met;
        }
    }
    
    public class PlaythroughSimulator
    {
        readonly VirtualPlayer _player;
        readonly RunSimulator _runSimulator;
        readonly PlaythroughEndConditions _endConditions;
        readonly SimulationSequnceContextProvider _contextProvider;

        public PlaythroughSimulator(VirtualPlayer player, RunSimulator runSimulator, PlaythroughEndConditions endConditions)
        {
            _player = player ?? throw new System.ArgumentNullException(nameof(player));
            _runSimulator = runSimulator ?? throw new System.ArgumentNullException(nameof(runSimulator));
            _endConditions = endConditions;
            _contextProvider = new SimulationSequnceContextProvider(_player.Context);
        }        
        
        public PlaythroughData Simulate()
        {
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
                endRichedConditions = _endConditions.ConditionsThatMet(lastResult, PlaythroughData.CombineTime(results));
            }while(!endRichedConditions.Any());
            
            return new PlaythroughData(results, _player.HeaderString, _player.Context.UpgradesPerIteration, endRichedConditions);
        }
    }
}