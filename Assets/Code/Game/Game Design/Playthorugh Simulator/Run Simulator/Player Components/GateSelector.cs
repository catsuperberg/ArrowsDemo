using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public enum GateSelectors
    {
        PerfectPlayer,
        GoodPlayer,
        AveragePlayer,
        BadPlayer,
        CustomPlayer
    }
    
    public static class GateSelectorsExtension
    {
        public static Dictionary<GateSelectors, float> GradeChances = new Dictionary<GateSelectors, float>(){
                    {GateSelectors.PerfectPlayer, 0},
                    {GateSelectors.GoodPlayer, 0.001f},
                    {GateSelectors.AveragePlayer, 0.012f},
                    {GateSelectors.BadPlayer, 0.09f}};    
        
        public static float Chance(this GateSelectors enumValue) 
        {
            return GradeChances[enumValue];
        }
    }
    
    public static class GateSelectorGrades
    {
        static Dictionary<int, float> _gradeFrequencies = new Dictionary<int, float>(){
                    {3, GateSelectors.GoodPlayer.Chance()},
                    {5, GateSelectors.AveragePlayer.Chance()},
                    {2, GateSelectors.BadPlayer.Chance()}};                
                
        public static float GetRandomGradeChance(Random rand)
            => WeightedRandom.NextFrom(_gradeFrequencies, rand);
            
        public static GateSelectors Grade(float chance)
        {
            if(GateSelectorsExtension.GradeChances.ContainsValue(chance))  
                return GateSelectorsExtension.GradeChances.First(kvp => kvp.Value == chance).Key;
            return GateSelectors.CustomPlayer;
        }
    }
    
    public class GateSelector
    {
        float skillProgressCoeff = 0.98f;
        public GateSelectors Grade 
            => GateSelectorGrades.Grade(_chanceOfWorseChoice);
        float _chanceOfWorseChoice; 
        float GetChance {get
            {
                var oldValue = _chanceOfWorseChoice;
                _chanceOfWorseChoice *= skillProgressCoeff;
                return oldValue;
            }} 
        readonly Random _random = new Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());

        public GateSelector(float chanceOfWorseChoice, bool skillProgression = true)
        {
            _chanceOfWorseChoice = chanceOfWorseChoice;
            skillProgressCoeff = skillProgression ? skillProgressCoeff : 1f;
        }
        
        public OperationInstance Choose(OperationPair pair, BigInteger initialValue)
        {
            var chanceCheck = _random.NextDouble();
            return (chanceCheck >= GetChance) ? pair.BestOperation(initialValue) : pair.WorseOperation(initialValue);
        }
    }
}