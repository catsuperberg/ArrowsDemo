using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfildComponents;
using System.Numerics;

namespace Game.Gameplay.Realtime
{
    public interface IRuntimeFactory
    {
        public Playfield GetLevel();
        public Runthrough GetRunthrough(Playfield level);
    }
}