using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public interface ISequenceManager
    {
        public SequenceContext GetContext();  // HACK should go to meta domain instead of sequence manager
        public BigInteger GetNextTargetScore();
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread);
    }    
}