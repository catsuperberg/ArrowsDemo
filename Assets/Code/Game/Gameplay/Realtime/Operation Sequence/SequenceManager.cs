using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class SequenceManager : ISequenceManager//, IStateReportableProcess
    {        
        ISequenceCalculator _meta;
        IContextProvider _runContextProvider;
                
        const int _numIterationsForAverage = 2000;
                
        public SequenceManager(ISequenceCalculator meta, IContextProvider runContextProvider)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to MetaManager");
                
            if(runContextProvider == null)
                throw new System.Exception("IContextProvider isn't provided to RuntimeFactory");
                
            _runContextProvider = runContextProvider;                
            _meta = meta;
        }
        
        public BigInteger GetNextTargetScore()
        {       
            var target = GenerateAverageTarget();                                                   
            return target;
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread)
        {
            var sequence = _meta.GenerateSequence(targetScore, spread, _runContextProvider.GetContext());            
            return sequence;
        }
        
        BigInteger GenerateAverageTarget()
        {
            return _meta.GetAverageSequenceResult(_runContextProvider.GetContext(), _numIterationsForAverage);
        }
    }
}