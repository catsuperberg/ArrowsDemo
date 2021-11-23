using System.Collections.Generic;
using System.Numerics;
using Sequence;

namespace GameMeta
{
    namespace Operation
    {
        public class SequenceGenerator
        {     
            PairGenerator _pairGenerator;
            OperationExecutor _exec;
            
            public SequenceGenerator(PairGenerator pairGenerator, OperationExecutor exec)
            {                
                if(pairGenerator == null)
                    throw new System.Exception("PairGenerator not provided to SequenceGenerator");
                if(exec == null)
                    throw new System.Exception("OperationExecutor not provided to SequenceGenerator");
            
                _pairGenerator = pairGenerator;
                _exec = exec;
            }
            
            public OperationPairsSequence GetSequenceWithRandomPairs(int SequenceLength)
            {
                List<OperationPair> newSequence = new List<OperationPair>();
                for(int i = 0; i < SequenceLength; i++)
                    newSequence.Add(_pairGenerator.Generate());
                return new OperationPairsSequence(newSequence);
            }
            
            public BigInteger CalculateBestResult(List<OperationPair> sequence,
                int initialValue)
            {                
                BigInteger result = new BigInteger(initialValue);
                foreach(OperationPair pair in sequence)
                {
                    result = _exec.Perform(pair.BestOperation(), result);
                    if(result < 1)
                        return -1;
                }
                return result;
            }
        }    
    }
}