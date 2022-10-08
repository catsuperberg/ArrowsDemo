using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class SequenceGenerator
    {     
        OperationExecutor _exec;
        Random _rand;           
        OperationFactory _opertionFactory;
        
        
        int _cacheSize = 60;
        OperationPair[] _pairCache;
        int _currentPairIndex;
        OperationPair NextInstance() 
        {
            var index = ++_currentPairIndex;
            if(index >= _cacheSize)
                _currentPairIndex = 0;
            return _pairCache[_currentPairIndex];
        }
        
        public SequenceGenerator(OperationExecutor exec, OperationFactory operationFactory)
        {                
            _exec = exec ?? throw new ArgumentNullException(nameof(exec));
            _opertionFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory));
            _rand = new Random(this.GetHashCode());
            
            FillCache(_cacheSize);
        }
        
        void FillCache(int length)
        {
            _cacheSize = length;
            _pairCache = Enumerable.Range(1, _cacheSize)
                .Select(entry => new OperationPair(_opertionFactory))
                .ToArray();
        }
                
        public OperationPairsSequence GetSequenceWithRandomPairsOptimized(int length, int initValue = 1)
        {            
            if(length > _cacheSize)
                FillCache(length);
                
            var sequence = _pairCache.Take(length).ToArray();
            sequence.ToList().ForEach(entry => entry.Regenerate());
                
            // var sequence = Enumerable.Range(1, length)
            //     .Select(entry => NextInstance())
            //     .ToArray();
            var results = CalculateResults(sequence, new BigInteger(initValue));
            while(results.Any(entry => entry <= 0))
                ChangeInvalidPair(sequence, results, new BigInteger(initValue));                  
                
            return new OperationPairsSequence(sequence, results.Last());            
        }
        
        // public IEnumerable<OperationPairsSequence> GetSequences(int length, int sequenceCount, int initValue = 1)
        // {
            
        //     var finishedSequences = new List<OperationPairsSequence>();
        //     var sequences = Enumerable.Range(1, sequenceCount)
        //         .Select(entry => GenerateSequence(length));
                           
        //     foreach(var sequence in sequences)
        //     {                
        //         var results = CalculateResults(sequence, new BigInteger(initValue));
        //         while(results.Any(entry => entry <= 0))
        //             ChangeInvalidPair(sequence, results, new BigInteger(initValue));  
        //         finishedSequences.Add(new OperationPairsSequence(sequence, results.Last()));
        //     }            
                
        //     return finishedSequences;            
        // }
        
        // OperationPair[] GenerateSequence(int length)
        //     => Enumerable.Range(1, length)
        //         .Select(entry => new OperationPair(_opertionFactory))
        //         .ToArray();
        
        void ChangeInvalidPair(OperationPair[] sequence, List<BigInteger> results, BigInteger initValue)
        {
            // var invalidResultAt = results.TakeWhile(entry => entry > 0).Count();
            var invalidResultAt = 0;
            foreach(var result in results)
            {
                if(result <= 0)
                    break;
                invalidResultAt++;
            }
            
            var pair = sequence[invalidResultAt];
            var resultBeforePair = (invalidResultAt > 0) ? results[invalidResultAt-1] : initValue; 
            BigInteger resultAfterCorrection;
            OperationInstance tempOperation;
            do 
            {                        
                tempOperation = _opertionFactory.GetRandom();
                resultAfterCorrection = _exec.Perform(tempOperation, resultBeforePair);
            } while (resultAfterCorrection < 1); // reroll if best choice gives result less than 1 
            sequence[invalidResultAt] = new OperationPair(pair.LeftOperation, tempOperation);
                       
            if(invalidResultAt == 0)
            {
                results[invalidResultAt] = sequence[invalidResultAt].BestOperationResult(initValue, _exec); 
                invalidResultAt++;
            }
                
                       
            for(int index = invalidResultAt; index < results.Count; index++)
                results[index] = sequence[index].BestOperationResult(results[index-1], _exec);              
        }     
        
        List<BigInteger> CalculateResults(OperationPair[] sequence, BigInteger initValue)
        {            
            var accumulator = initValue;
            
            return sequence.Select(entry => accumulator = entry.BestOperationResult(accumulator, _exec)).ToList(); 
        }
        
        
            
            // IEnumerable<OperationPair> sequence;
            // IEnumerable<BigInteger> results; 
            // do
            // {
            //     sequence = Enumerable.Range(1, SequenceLength)
            //         .Select(entry => new OperationPair(_opertionFactory));
            //     var accumulator = new BigInteger(initValue);
            //     results = sequence.Select(enrtry => accumulator = enrtry.BestOperationResult(accumulator, _exec)); 
            // } while (results.Any(entry => entry <= 0));            
            // return new OperationPairsSequence(sequence, results.Last());   
    }    
}