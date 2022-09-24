using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class SequenceGenerator
    {     
        OperationExecutor _exec;
        Random _rand;           
        OperationFactory _opertionFactory;
        
        public SequenceGenerator(OperationExecutor exec, OperationFactory operationFactory)
        {                
            _exec = exec ?? throw new ArgumentNullException(nameof(exec));
            _opertionFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory));
            _rand = new Random(this.GetHashCode());
        }
        
        public OperationPairsSequence GetSequenceWithRandomPairs(int SequenceLength, int initValue = 1)
        {
            var result = new BigInteger(initValue);
            var tempResult = result;
            OperationPair pair;
            List<OperationPair> newSequence = new List<OperationPair>();
            for(int i = 0; i < SequenceLength; i++)
            {                    
                do
                {
                    tempResult = result;
                    pair = new OperationPair(_opertionFactory);
                    tempResult = _exec.Perform(pair.BestOperation(tempResult, _exec), tempResult);                   
                } while (tempResult < 1); // reroll if best choice can be less than 1
                result = tempResult;
                newSequence.Add(pair);
            }
            
            return new OperationPairsSequence(newSequence, result);
        }
        
        public OperationPairsSequence GetSequenceWithRandomPairsOptimized(int SequenceLength, int initValue = 1)
        {
            var result = new BigInteger(initValue);
            var tempResult = result;
            OperationPair pair;
            List<OperationPair> newSequence = new List<OperationPair>();
            for(int i = 0; i < SequenceLength; i++)
            {
                pair = new OperationPair(_opertionFactory);
                tempResult = pair.BestOperationResult(tempResult, _exec);
                if(tempResult < 1)
                {
                    OperationInstance tempOperation;
                    do 
                    {                        
                        tempResult = result;
                        tempOperation = _opertionFactory.GetRandom();
                        tempResult = _exec.Perform(tempOperation, tempResult);
                    } while (tempResult < 1); // reroll if best choice gives result less than 1 
                    var operations = new OperationInstance[] {pair.LeftOperation, tempOperation};
                    _rand.Shuffle(operations);
                    pair = new OperationPair(operations[0], operations[1]);
                }
                
                newSequence.Add(pair);
                result = tempResult;
            }
            return new OperationPairsSequence(newSequence, result);
        }        
        
        public BigInteger CalculateBestResult(List<OperationPair> sequence, int initialValue)
        {                
            BigInteger result = new BigInteger(initialValue);
            foreach(OperationPair pair in sequence)
                result = _exec.Perform(pair.BestOperation(initialValue,  _exec), result);
            return result;
        }
    }    
}