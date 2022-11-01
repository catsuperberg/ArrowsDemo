using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Game.GameDesign
{
    public class PlaythroughEndConditions
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

        public bool Met(RunData data, TimeSpan combinedTime)
        {
            var conditionScore = data.FinalScore >= _maxReward;
            var conditionGameplayTime = data.GameplayTime >= _gameplayTimeOfRun;
            var conditionPlayTime = combinedTime >= _playTime;
            return conditionScore || conditionPlayTime || conditionPlayTime;
        }
    }
    
    public class PlaythroughSimulator
    {
        const int _endPlaythroughGateCount = 50;    
    
        readonly VirtualPlayer _player;
        readonly RunSimulator _runSimulator;
        readonly PlaythroughEndConditions _endConditions;
        readonly SimulationSequnceContextProvider _contextProvider;

        public PlaythroughSimulator(VirtualPlayer player, RunSimulator runSimulator, PlaythroughEndConditions endConditions)
        {
            _player = player ?? throw new System.ArgumentNullException(nameof(player));
            _runSimulator = runSimulator ?? throw new System.ArgumentNullException(nameof(runSimulator));
            _endConditions = endConditions ?? throw new ArgumentNullException(nameof(endConditions));
            _contextProvider = new SimulationSequnceContextProvider(_player.Context);
        }        
        
        public PlaythroughData Simulate()
        {
            var results = new List<RunData>();
            RunData lastResult;
            SequenceContext sequenceContext = null;
            do
            {
                sequenceContext = _player.Context.SequenceContext;
                lastResult = _runSimulator.Simulate(sequenceContext, _player.Actors);
                _player.RecieveReward(lastResult.FinalScore);                
                _player.BuyUpgrades();
                results.Add(lastResult);
            }while(!_endConditions.Met(lastResult, PlaythroughData.CombineTime(results)));
            
            return new PlaythroughData(results);
        }
    }
}