using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public interface ISequenceCalculator
    {
        OperationPairsSequence GetSequenceInSpreadRange(BigInteger targetMaxResult, int spreadPercentage,
            SequenceContext context);
        BigInteger GetAverageSequenceResult(SequenceContext context);
        OperationPairsSequence GetRandomSequence(SequenceContext context);
    }    
}
