using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class SequenceGenerator
    {     
        Random _rand;           
        OperationFactory _opertionFactory;
                
        // int _cacheSize = 1;
        // OperationPair[] _pairCache;
        // int _currentPairIndex;
        // OperationPair NextPair() 
        // {
        //     var index = ++_currentPairIndex;
        //     if(index >= _cacheSize)
        //         _currentPairIndex = 0;
        //     return _pairCache[_currentPairIndex];
        // }
        
        public SequenceGenerator(OperationFactory operationFactory)
        {                
            _opertionFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory));
            _rand = new Random(this.GetHashCode());
            
            // FillCache(_cacheSize);
        }        
        
        public OperationPairsSequence GetRandomSequence(int length, int initValue = 1)
        {                 
            // FillCache(length);
            // var previousResult = new BigInteger(initValue);
            // BigInteger tempResult;
            // var sequence = _pairCache.Take(length).ToList();
            
            var previousResult = new BigInteger(initValue);
            BigInteger tempResult;
            var sequence = _opertionFactory.GetInitialSequence(length);
            for(int i = 0; i < length; i++)
            {
                tempResult = sequence[i].BestOperationResult(previousResult);
                while(tempResult <= 0)
                {
                    sequence[i] = _opertionFactory.GetRandomPair();
                    tempResult = sequence[i].BestOperationResult(previousResult);
                }
                previousResult = tempResult;
            }
            
            return new OperationPairsSequence(sequence, previousResult);
        }      
        
        // void FillCache(int length)
        // {
        //     _cacheSize = length;
        //     _pairCache = new OperationPair[_cacheSize];
        //     for(int i = 0; i < _cacheSize; i++) _pairCache[i] = _opertionFactory.GetRandomPair();
        //     // _pairCache = Enumerable.Range(1, _cacheSize)
        //     //     .Select(entry => _opertionFactory.GetRandomPair())
        //     //     .ToArray();
        // }
        
        // void ChangeInvalidPair(OperationPair[] sequence, List<BigInteger> results, BigInteger initValue)
        // {
        //     var invalidResultAt = 0;
        //     foreach(var result in results)
        //     {
        //         if(result <= 0)
        //             break;
        //         invalidResultAt++;
        //     }
            
        //     var pair = sequence[invalidResultAt];
        //     var resultBeforePair = (invalidResultAt > 0) ? results[invalidResultAt-1] : initValue; 
        //     BigInteger resultAfterCorrection;
        //     OperationInstance tempOperation;
        //     do 
        //     {                        
        //         tempOperation = _opertionFactory.GetRandom();
        //         resultAfterCorrection = tempOperation.Perform(resultBeforePair);
        //     } while (resultAfterCorrection < 1); // reroll if best choice gives result less than 1 
        //     sequence[invalidResultAt] = new OperationPair(pair.LeftOperation, tempOperation);
                       
        //     if(invalidResultAt == 0)
        //     {
        //         results[invalidResultAt] = sequence[invalidResultAt].BestOperationResult(initValue); 
        //         invalidResultAt++;
        //     }
                
                       
        //     for(int index = invalidResultAt; index < results.Count; index++)
        //         results[index] = sequence[index].BestOperationResult(results[index-1]);              
        // }     
        
        // List<BigInteger> CalculateResults(OperationPair[] sequence, BigInteger initValue)
        // {            
        //     var accumulator = initValue;
            
        //     return sequence.Select(entry => accumulator = entry.BestOperationResult(accumulator)).ToList(); 
        // }
    }    
}