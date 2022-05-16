using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents;

namespace Game.GameState
{
    public interface IAppStateFactory
    {
        public IPreRun GetPreRun(bool skipToRun);
        public Runthrough GetRunthrough(RunthroughContext runContext);
        public IPostRun GetPostRun(RunFinishContext FinishContext);
        public AdState GetAd();
    }
}