using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public static class SequenceGenerator
    {             
        public static OperationPairsSequence GetRandomSequence(int length, OperationFactory operationFactory, int initValue = 1)
        {                 
            var previousResult = new BigInteger(initValue);
            BigInteger tempResult;
            var sequence = operationFactory.GetInitialSequence(length);
            for(int i = 0; i < length; i++)
            {
                tempResult = sequence[i].BestOperationResult(previousResult);
                while(tempResult <= 0)
                {
                    sequence[i] = operationFactory.GetRandomPair();
                    tempResult = sequence[i].BestOperationResult(previousResult);
                }
                previousResult = tempResult;
            }
            
            return new OperationPairsSequence(sequence, previousResult);
        }      
    }    
}