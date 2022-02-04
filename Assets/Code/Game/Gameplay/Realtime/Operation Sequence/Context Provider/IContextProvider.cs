using Game.Gameplay.Realtime.OperationSequence.Operation;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public interface IContextProvider
    {
        public SequenceContext getContext();
    }
}