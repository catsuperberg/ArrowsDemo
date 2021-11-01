using System.Collections.Generic;
using System.Numerics;

namespace Sequence
{
    public class OperationPairsSequence
    {
        public List<OperationPair> Sequence {get; private set;}
        // public BigInteger MaxResult {get; private set;}
        
        public OperationPairsSequence(List<OperationPair> sequence)
        {
            if(sequence.Count == 0)
                throw new System.Exception("OperationPairsSequence not provided with List of <OperationPair>");
            
            Sequence = sequence;
            // MaxResult = bestResult;
        }
    }
}