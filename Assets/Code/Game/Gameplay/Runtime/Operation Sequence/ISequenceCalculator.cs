using Game.Gameplay.Runtime.OperationSequence.Operation;
using System.Numerics;

namespace Game.Gameplay.Runtime.OperationSequence
{
    public interface ISequenceCalculator
    {
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context);
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations);
    }    
}
