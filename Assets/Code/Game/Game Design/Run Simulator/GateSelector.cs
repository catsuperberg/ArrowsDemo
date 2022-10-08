using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Game.GameDesign
{
    public enum GateSelectors
    {
        PerfectPlayer,
        GoodPlayer,
        AveragePlayer,
        BadPlayer
    }
    
    public static class GateSelectorsExtension
    {
        public static float Chance(this GateSelectors enumValue) 
        {
            switch (enumValue) 
            {
                case GateSelectors.PerfectPlayer: return 0;
                case GateSelectors.GoodPlayer: return 0.001f;
                case GateSelectors.AveragePlayer: return 0.012f;
                case GateSelectors.BadPlayer: return 0.15f;
                default: return 0;
            }
        }
    }
    
    public static class GateSelectorGrades
    {
        static Dictionary<int, float> _gradeFrequencies = new Dictionary<int, float>(){
                    {3, GateSelectors.GoodPlayer.Chance()},
                    {5, GateSelectors.AveragePlayer.Chance()},
                    {2, GateSelectors.BadPlayer.Chance()}};                
                
        public static float GetRandomGrade()
            => WeightedRandom.NextFrom(_gradeFrequencies);
    }
    
    public class GateSelector
    {
        float _chanceOfWorseChoice;
        readonly OperationExecutor _exec;        
        readonly Random _random = new Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());

        public GateSelector(float chanceOfWorseChoice, OperationExecutor exec)
        {
            _chanceOfWorseChoice = chanceOfWorseChoice;
            _exec = exec ?? throw new System.ArgumentNullException(nameof(exec));
        }
        
        public OperationInstance Choose(OperationPair pair, BigInteger initialValue)
        {
            var chanceCheck = _random.NextDouble();
            return (chanceCheck >= _chanceOfWorseChoice) ? pair.BestOperation(initialValue, _exec) : pair.WorseOperation(initialValue, _exec);
        }
    }
}