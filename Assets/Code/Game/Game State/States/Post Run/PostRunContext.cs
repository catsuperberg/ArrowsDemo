using System.Numerics;

namespace Game.GameState.Context
{
    public class PostRunContext
    {
        public readonly BigInteger SelectedReward;
        public readonly bool ShowAdBeforeApplyingReward;

        public PostRunContext(BigInteger playerSelectedReward, bool showAdBeforeApplyingReward)
        {
            SelectedReward = playerSelectedReward;
            ShowAdBeforeApplyingReward = showAdBeforeApplyingReward;
        }
    }
}