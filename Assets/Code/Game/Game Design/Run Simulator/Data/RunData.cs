using System;
using System.Numerics;

namespace Game.GameDesign
{
    public class RunData
    {
        public readonly BigInteger TargetScore;
        public readonly BigInteger BestPossibleResult;
        public readonly BigInteger FinalScore;
        public readonly TimeSpan GameplayTime;
        public readonly TimeSpan AdSeconds;
        public readonly TimeSpan CombinedTime;

        public RunData(BigInteger targetScore, BigInteger bestPossibleResult, BigInteger finalScore, float gameplaySeconds, float adSeconds = 0)
        {
            TargetScore = targetScore;
            BestPossibleResult = bestPossibleResult;
            FinalScore = finalScore;
            GameplayTime = TimeSpan.FromSeconds(gameplaySeconds);
            AdSeconds = TimeSpan.FromSeconds(adSeconds);
            CombinedTime = GameplayTime+AdSeconds;
        }
    }
}