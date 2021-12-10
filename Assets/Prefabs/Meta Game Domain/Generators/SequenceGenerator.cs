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
            
            public OperationPairsSequence GetSequenceWithRandomPairs(int SequenceLength, int initValue = 1)
            {
                var result = new BigInteger(initValue);
                var tempResult = new BigInteger(0);
                OperationPair pair;
                List<OperationPair> newSequence = new List<OperationPair>();
                for(int i = 0; i < SequenceLength; i++)
                {                    
                    do
                    {
                        tempResult = result;
                        pair = _pairGenerator.Generate();    
                        tempResult = _exec.Perform(pair.BestOperation(initValue, _exec), tempResult);                   
                    } while (tempResult < 1); // reroll if best choice can be less than 1
                    result = tempResult;
                    newSequence.Add(pair);
                }
                return new OperationPairsSequence(newSequence);
            }
            
            public BigInteger CalculateBestResult(List<OperationPair> sequence,
                int initialValue)
            {                
                BigInteger result = new BigInteger(initialValue);
                foreach(OperationPair pair in sequence)
                {
                    result = _exec.Perform(pair.BestOperation(initialValue,  _exec), result);
                    // if(result < 1)
                    //     return -1; // discard sequence if at some point less than 1
                }
                return result;
            }
        }    
    }
}