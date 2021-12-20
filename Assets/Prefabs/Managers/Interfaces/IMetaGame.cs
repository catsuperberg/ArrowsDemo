using Sequence;
using System.Numerics;

namespace GameMeta
{
    public interface IMetaGame
    {
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context);
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations);
    }    
}
