using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class SequenceManager : ISequenceManager//, IStateReportableProcess
    {        
        ISequenceCalculator _meta;
        ISequenceContextProvider _runContextProvider;
                                
        public SequenceManager(ISequenceCalculator meta, ISequenceContextProvider runContextProvider)
        {                
            _runContextProvider = runContextProvider ?? throw new System.ArgumentNullException(nameof(runContextProvider));                
            _meta = meta ?? throw new System.ArgumentNullException(nameof(meta));
        }
        
        public BigInteger GetNextTargetScore()
        {       
            var target = GenerateAverageTarget();                                                   
            return target;
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread)
        {
            var sequence = _meta.GetSequenceInSpreadRange(targetScore, spread, _runContextProvider.GetContext());            
            return sequence;
        }
        
        BigInteger GenerateAverageTarget()
        {
            return _meta.GetAverageSequenceResult(_runContextProvider.GetContext());
        }
    }
}