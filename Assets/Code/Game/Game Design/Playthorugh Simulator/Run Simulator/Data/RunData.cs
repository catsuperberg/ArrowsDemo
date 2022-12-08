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
        public readonly TimeSpan LevelRunTime;
        public readonly TimeSpan AdSeconds;
        public readonly TimeSpan CombinedTime;
        public readonly GateChoices GateDecisions;
        public readonly float AdMultiplier;

        public RunData(
            BigInteger targetScore, BigInteger bestPossibleResult, BigInteger finalScore,
            float gameplaySeconds, float levelRunSeconds, GateChoices gateDecisions,
            float adMultiplier, float adSeconds = 0)
        {
            TargetScore = targetScore;
            BestPossibleResult = bestPossibleResult;
            FinalScore = finalScore;
            GameplayTime = TimeSpan.FromSeconds(gameplaySeconds);
            AdSeconds = TimeSpan.FromSeconds(adSeconds);
            CombinedTime = GameplayTime + AdSeconds;
            LevelRunTime = TimeSpan.FromSeconds(levelRunSeconds);
            GateDecisions = gateDecisions ?? throw new ArgumentNullException(nameof(gateDecisions));
            AdMultiplier = adMultiplier;
        }
    }
}