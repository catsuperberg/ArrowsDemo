using System.Numerics;

namespace Game.GameDesign
{
    public class SimulationData
    {
        public readonly BigInteger TargetScore;
        public readonly BigInteger BestPossibleResult;
        public readonly BigInteger FinalScore;
        public readonly float GameplaySeconds;
        public readonly float AdSeconds;
        public float CombinedSeconds {get => GameplaySeconds+AdSeconds;}

        public SimulationData(BigInteger targetScore, BigInteger bestPossibleResult, BigInteger finalScore, float gameplaySeconds, float adSeconds = 0)
        {
            TargetScore = targetScore;
            BestPossibleResult = bestPossibleResult;
            FinalScore = finalScore;
            GameplaySeconds = gameplaySeconds;
            AdSeconds = adSeconds;
        }
    }
}