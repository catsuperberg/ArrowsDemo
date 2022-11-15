using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;
using System.Collections.Generic;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public interface ISequenceCalculator
    {
        OperationPairsSequence GetSequenceInSpreadRange(BigInteger targetMaxResult, int spreadPercentage,
            SequenceContext context);
        BigInteger GetAverageSequenceResult(SequenceContext context);
        // BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations);
        OperationPairsSequence GetRandomSequence(SequenceContext context);
    }    
}
