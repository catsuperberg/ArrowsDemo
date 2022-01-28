using Game.GameState;
using Game.Gameplay.Runtime.OperationSequence.Operation;
using System.Numerics;

namespace Game.Gameplay.Runtime.OperationSequence
{
    public interface ISequenceManager
    {
        public SequenceContext GetContext();  // HACK should go to meta domain instead of sequence manager
        public BigInteger GetNextTargetScore();
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread);
    }    
}