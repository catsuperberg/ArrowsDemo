using System.Numerics;

namespace Game.Gameplay.Realtime
{
    public class RunFinishContext
    {
        public readonly BigInteger RewardForTheRun;

        public RunFinishContext(BigInteger rewardForTheRun)
        {
            RewardForTheRun = rewardForTheRun;
        }
    }
}