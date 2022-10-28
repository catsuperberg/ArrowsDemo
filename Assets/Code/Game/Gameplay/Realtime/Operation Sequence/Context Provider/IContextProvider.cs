using Game.Gameplay.Realtime.OperationSequence.Operation;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public interface ISequenceContextProvider
    {
        public SequenceContext GetContext();
    }
}