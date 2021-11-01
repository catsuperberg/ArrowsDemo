using System.Numerics;
using Sequence;

namespace GamePlay
{
    public interface IMetaGame
    {
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context);
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations);
    }    
}
