using Sequence;
using System.Numerics;

namespace Level
{
    public interface ILevelManager
    {
        public void InitializeLevel(SequenceContext context, OperationPairsSequence sequence, BigInteger targetResult);
    }
}
    