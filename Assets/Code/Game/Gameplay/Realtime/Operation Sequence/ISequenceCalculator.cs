using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;
using System.Collections.Generic;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public interface ISequenceCalculator
    {
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int spreadPercentage,
            SequenceContext context);
        public BigInteger GetAverageSequenceResult(SequenceContext context);
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations);
    }    
}
