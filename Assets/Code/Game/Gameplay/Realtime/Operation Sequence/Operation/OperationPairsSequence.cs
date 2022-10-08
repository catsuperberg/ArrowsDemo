using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationPairsSequence
    {
        public readonly BigInteger BestPossibleResult;
        public IReadOnlyCollection<OperationPair> Sequence {get {return _sequence.ToList().AsReadOnly();}}
        private IEnumerable<OperationPair> _sequence;
        
        public OperationPairsSequence(List<OperationPair> sequence, BigInteger generationTimeResult)
        {
            if(sequence.Count == 0)
                throw new System.Exception("OperationPairsSequence not presented with List of <OperationPair>");
            
            _sequence = sequence;            
            BestPossibleResult = generationTimeResult;
        }
        
        public OperationPairsSequence(IEnumerable<OperationPair> sequence, BigInteger generationTimeResult)
        {
            if(!sequence.Any())
                throw new System.Exception("OperationPairsSequence not presented with List of <OperationPair>");
                
            _sequence = sequence;            
            BestPossibleResult = generationTimeResult;
        }
    }
}