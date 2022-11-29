using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public enum EndCondition
    {
        Reward,
        GameplayTime,
        PlayTime
    }
    
    public class CompletionConditions
    {       
        public struct Controlls
        {
            public float PlayMinutes;
            public float LongestRunMinutes;
            public BigInteger MaxReward;
            
            public static Controlls GetDefault()
                => new Controlls(0, 0, BigInteger.Zero);
                
            public Controlls(float playMinutes, float longestRunMinutes, BigInteger maxReward)
            {
                PlayMinutes = playMinutes;
                LongestRunMinutes = longestRunMinutes;
                MaxReward = maxReward;
            }
            
            public CompletionConditions ToConditions()
            {
                var playTime = TimeSpan.FromMinutes(PlayMinutes);
                var runTime = TimeSpan.FromMinutes(LongestRunMinutes);
                return new CompletionConditions(playTime, runTime, MaxReward);
            }
        }
        
        public readonly TimeSpan PlayTime;
        public readonly TimeSpan LongestRunTime;
        public readonly BigInteger RewardLimit;

        public CompletionConditions(TimeSpan playTime, TimeSpan runTime, BigInteger maxReward)
        {
            PlayTime = playTime;
            LongestRunTime = runTime;
            RewardLimit = maxReward;
        }
        
        public CompletionConditions(CompletionConditions source, TimeSpan? playTime = null, TimeSpan? runTime = null, BigInteger? maxReward = null)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            PlayTime = playTime ?? source.PlayTime;
            LongestRunTime = runTime ?? source.LongestRunTime;
            RewardLimit = maxReward ?? source.RewardLimit;
        }

        public bool MetAny(RunData data, TimeSpan combinedTime)
        {
            var met = ConditionsThatMet(data, combinedTime);
            return met.Any();
        }
        
        public IEnumerable<EndCondition> ConditionsThatMet(RunData data, TimeSpan combinedTime)
        {
            var met = new List<EndCondition>();
            if(data.FinalScore >= RewardLimit) met.Add(EndCondition.Reward);
            if(data.LevelRunTime >= LongestRunTime) met.Add(EndCondition.GameplayTime);
            if(combinedTime >= PlayTime) met.Add(EndCondition.PlayTime);
            return met;
        }
    }
}