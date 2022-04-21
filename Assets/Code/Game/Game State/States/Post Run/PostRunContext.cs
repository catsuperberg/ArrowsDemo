using System.Numerics;

namespace Game.GameState.Context
{
    public class PostRunContext
    {
        public readonly BigInteger SelectedReward;
        public readonly bool ShowAdBeforeApplyingReward;
        public readonly bool RestartInsteadOfMenu;

        public PostRunContext(BigInteger playerSelectedReward, bool showAdBeforeApplyingReward, bool restartInsteadOfMenu)
        {
            SelectedReward = playerSelectedReward;
            ShowAdBeforeApplyingReward = showAdBeforeApplyingReward;
            RestartInsteadOfMenu = restartInsteadOfMenu;
        }
    }
}