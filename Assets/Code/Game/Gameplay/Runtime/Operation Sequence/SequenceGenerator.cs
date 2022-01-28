using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Runtime.OperationSequence.Operation
{
    public class SequenceGenerator
    {     
        OperationExecutor _exec;
        Random _rand;   
        
        public SequenceGenerator(OperationExecutor exec)
        {                
            if(exec == null)
                throw new System.Exception("OperationExecutor not provided to SequenceGenerator");
        
            _exec = exec;
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
                    pair = new OperationPair();
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
                pair = new OperationPair();
                tempResult = pair.BestOperationResult(tempResult, _exec);
                if(tempResult < 1)
                {
                    OperationInstance tempOperation;
                    do 
                    {                        
                        tempResult = result;
                        tempOperation = new OperationInstance();
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
        
        // public OperationPairsSequence GetSequenceWithRandomPairs(int SequenceLength, int initValue = 1)
        // {
        //     var result = new BigInteger(initValue);
        //     var tempResult = result;
        //     List<OperationPair> newSequence = new List<OperationPair>();
        //     for(int i = 0; i < SequenceLength; i++)
        //     {                    
        //         newSequence.Add(new OperationPair());
        //     }
        //     for(int i = 0; i < SequenceLength; i++)
        //     {
        //         tempResult = _exec.Perform(newSequence[i].BestOperation(initValue, _exec), tempResult);
        //         if(tempResult < 1)
        //         {
        //             OperationInstance tempOperation;
        //             do 
        //             {                        
        //                 tempResult = result;
        //                 tempOperation = new OperationInstance();
        //                 tempResult = _exec.Perform(tempOperation, tempResult);
        //             } while (tempResult < 1); // reroll if best choice can be less than 1 
        //             newSequence[i] = new OperationPair(newSequence[i].LeftOperation, tempOperation); // TODO randomise side selection
        //         }
                
        //         result = tempResult;
        //     }
        //     return new OperationPairsSequence(newSequence, result);
        // }        
        
        
        public BigInteger CalculateBestResult(List<OperationPair> sequence, int initialValue)
        {                
            BigInteger result = new BigInteger(initialValue);
            foreach(OperationPair pair in sequence)
                result = _exec.Perform(pair.BestOperation(initialValue,  _exec), result);
            return result;
        }
    }    
}