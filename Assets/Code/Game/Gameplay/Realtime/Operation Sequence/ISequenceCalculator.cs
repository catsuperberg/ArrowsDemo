using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public interface ISequenceCalculator
    {
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context);
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations);
    }    
}
