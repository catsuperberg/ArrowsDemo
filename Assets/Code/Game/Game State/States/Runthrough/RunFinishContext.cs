using System.Numerics;

namespace Game.Gameplay.Realtime
{
    public class RunFinishContext
    {
        public readonly BigInteger RewardForTheRun;
        public readonly bool RunFailed;

        public RunFinishContext(BigInteger rewardForTheRun, bool runFailed)
        {
            RewardForTheRun = rewardForTheRun;
            RunFailed = runFailed;
        }
    }
}