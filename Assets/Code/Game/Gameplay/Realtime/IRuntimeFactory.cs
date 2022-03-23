using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents;
using System.Numerics;

namespace Game.Gameplay.Realtime
{
    public interface IRuntimeFactory
    {
        public RunthroughContext GetRunthroughContext();
    }
}