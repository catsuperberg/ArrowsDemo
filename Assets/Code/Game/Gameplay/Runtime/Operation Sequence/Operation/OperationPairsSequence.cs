using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Runtime.OperationSequence.Operation
{
    public class OperationPairsSequence
    {
        public List<OperationPair> Sequence {get; private set;}
        public readonly BigInteger ResultAtInitialGeneration;
        
        public OperationPairsSequence(List<OperationPair> sequence, BigInteger generationTimeResult)
        {
            if(sequence.Count == 0)
                throw new System.Exception("OperationPairsSequence not provided with List of <OperationPair>");
            
            Sequence = sequence;
            ResultAtInitialGeneration = generationTimeResult;
        }
    }
}