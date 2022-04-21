using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents;

namespace Game.GameState
{
    public interface IAppStateFactory
    {
        public PreRun GetPreRun();
        public Runthrough GetRunthrough(RunthroughContext runContext);
        public IPostRun GetPostRun(RunFinishContext FinishContext);
        public AdState GetAd();
    }
}