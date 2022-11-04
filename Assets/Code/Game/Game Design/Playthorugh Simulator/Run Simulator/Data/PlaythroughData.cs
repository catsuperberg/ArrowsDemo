using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Game.GameDesign
{
    public class PlaythroughData
    {
        public readonly IEnumerable<RunData> Runs;
        public readonly int NumberOfRuns;
        public readonly TimeSpan CombinedTime;
        public readonly string PlayerHeader;
        public readonly IEnumerable<EndCondition> FinishReasons;
        public readonly IReadOnlyCollection<int> UpgradesPerRun;
        public IReadOnlyCollection<TimeSpan> TimePerRun 
            {get => Runs.Select(entry => entry.CombinedTime).ToList().AsReadOnly();}
        public IReadOnlyCollection<TimeSpan> LevelTimePerRun 
            {get => Runs.Select(entry => entry.LevelRunTime).ToList().AsReadOnly();}
        public IReadOnlyDictionary<BigInteger, TimeSpan> TimeToRewards(IEnumerable<BigInteger> rewards) 
        {
            var ascendingReward = from reward in rewards orderby reward ascending select reward;  
            if(ascendingReward.Last() > Runs.Last().FinalScore)
                throw new ArgumentOutOfRangeException($"Largest reward: {ascendingReward.Last()} higher than best run: {Runs.Last().FinalScore}");
            return ascendingReward
                .ToDictionary(reward => reward, reward => Runs
                    .TakeWhile(run => run.FinalScore < reward)
                    .Aggregate(new TimeSpan(), (sum, run) => sum += run.CombinedTime));
        }
        
        public static List<BigInteger> LogarithmicRewardsList(BigInteger maxReward, int count)
        {
            var maxLog = BigInteger.Log10(maxReward);
            var logStep = maxLog/count;
            var baseValue = new BigInteger(10);		
            var rewards = Enumerable.Range(1, count-1).Select(multiplier => baseValue.PowFractional(logStep*multiplier)).ToList();
            rewards.Add(maxReward);
            return rewards;
        }

        public PlaythroughData(
            IEnumerable<RunData> playthroughRuns, string playerString, IReadOnlyCollection<int> upgradesPer, 
            IEnumerable<EndCondition> finishReasons)
        {
            Runs = playthroughRuns ?? throw new System.ArgumentNullException(nameof(playthroughRuns));
            NumberOfRuns = Runs.Count();
            CombinedTime = CombineTime(Runs);
            PlayerHeader = playerString;
            UpgradesPerRun = upgradesPer;
            FinishReasons = finishReasons;
        }
        
        static public TimeSpan CombineTime(IEnumerable<RunData> runs)
            => runs.Aggregate(new TimeSpan(0), (sum, entry) => sum += entry.CombinedTime);
    }
}