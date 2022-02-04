using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationPairsSequence
    {
        public readonly BigInteger ResultAtCreation;
        public IReadOnlyCollection<OperationPair> Sequence {get {return _sequence.AsReadOnly();}}
        private List<OperationPair> _sequence;
        
        public OperationPairsSequence(List<OperationPair> sequence, BigInteger generationTimeResult)
        {
            if(sequence.Count == 0)
                throw new System.Exception("OperationPairsSequence not provided with List of <OperationPair>");
            
            _sequence = sequence;            
            ResultAtCreation = generationTimeResult;
        }
    }
}