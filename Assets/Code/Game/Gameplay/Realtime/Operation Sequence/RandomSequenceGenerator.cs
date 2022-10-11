using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class RandomSequenceGenerator : ISequenceCalculator
    {       
        const int _numIterationsForAverage = 64;  
        const int _generationTimout = 3000;
        const int _medianLevels = 3;
        const int _medianGroupsPerLevel = 4; // From top 
        
        float _coefficient = 0.7f;
        OperationPairsSequence _sequence = null;
        int _CPU_count = 1;
        int _numThreads = 1;
        OperationFactory.Factory _opertionFactory;
        
        RandomSequenceGenerator(OperationFactory.Factory operationFactory)
        {
            _CPU_count = SystemInfo.processorCount;
            _numThreads = Mathf.Clamp((_CPU_count * 2) -1, 1, int.MaxValue);
            _opertionFactory = operationFactory ?? throw new System.ArgumentNullException(nameof(operationFactory));            
            
            var minimalMedianCount = (int)Mathf.Pow(_medianGroupsPerLevel, _medianLevels);
            if(minimalMedianCount > _numIterationsForAverage)
                throw new System.Exception($"Wouldn't be able to calculate median with set levels and count.\nMinimal count : {minimalMedianCount}");
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {        
            _sequence = null;
            CancellationTokenSource tokenSource= new CancellationTokenSource(); 
            CancellationToken ct = tokenSource.Token;                
            
            var generationTimout = new System.Timers.Timer();
            generationTimout.Elapsed += (caller, e) => {tokenSource.Cancel();};
            generationTimout.Interval = _generationTimout;
            generationTimout.Start();
            
            var operationFactories = Enumerable.Range(1, _numThreads).Select(entry => _opertionFactory.Create());  
            var tasks = new List<Task<OperationPairsSequence>>();
            foreach(var operationFactory in operationFactories)
            {
                var task = Task.Run(() => {return GenerateSequenceAsync(ct, targetMaxResult, SpreadPercentage, context, operationFactory);});
                tasks.Add(task);
            }
            Task.WaitAny(tasks.ToArray());            
            tokenSource.Cancel(); 
            generationTimout.Dispose();   
            _sequence = tasks.FirstOrDefault(entry => entry.Result != null).Result;
               
            if(_sequence == null)
                throw new System.Exception($"No sequence generated but trying to return.\n targetMaxResult was: {targetMaxResult}");         
            return _sequence;    
        }
        
        async Task<OperationPairsSequence> GenerateSequenceAsync(
            CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, 
            SequenceContext context, OperationFactory operationFactor)
        {
            float tempCoeff = _coefficient;
            var generator = new SequenceGenerator(operationFactor);
            OperationPairsSequence tempSequence;
            BigInteger result = new BigInteger(0);
            result += context.InitialValue;
            BigInteger spread = (new BigInteger(SpreadPercentage) * targetMaxResult)/new BigInteger(100);
            while(!token.IsCancellationRequested)
            {
                tempSequence = generator.GetRandomSequence(context.NumberOfOperations, context.InitialValue);  
                result = tempSequence.BestPossibleResult;  
                if(BigInteger.Abs(targetMaxResult - result) < spread)
                    return tempSequence;
            }
            
            return null;
        }
        
        public BigInteger GetAverageSequenceResult(SequenceContext context)
            => GetAverageSequenceResult(context, _numIterationsForAverage);
        
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations)
        {
            var iterationsPerThread = numberOfIterations/_numThreads;           
            // var operationFactories = Enumerable.Range(1, _numThreads).Select(entry => _opertionFactory.Create());     
            // var tasks = new List<Task<IEnumerable<BigInteger>>>();
            // foreach(var operationFactory in operationFactories)
            // {
            //     var task = Task.Run(() => {return ResultsOfIterationsAsync(context, iterationsPerThread, operationFactory);});
            //     tasks.Add(task);
            // }
            
            
             
            // var tasks = new List<Task<IEnumerable<BigInteger>>>();
            // foreach(var i in Enumerable.Range(1, _numThreads))
            // {
            //     var task = Task.Run(() => {return ResultsOfIterationsAsync(context, iterationsPerThread, _opertionFactory.Create());});
            //     tasks.Add(task);
            // }
            // Task.WaitAll(tasks.ToArray());
            // var allResults = tasks.SelectMany(entry => entry.Result);
            
            // var results = Enumerable.Range(1, _numThreads).Select(entry => ResultsOfIterationsAsync(context, iterationsPerThread, _opertionFactory.Create()).Result);
            
            var results = Enumerable.Range(1, _numThreads).AsParallel().Select(entry => ResultsOfIterationsAsync(context, iterationsPerThread, _opertionFactory.Create()).Result);
            var allResults = results.SelectMany(entry => entry);
            
            // return MathUtils.Median(allResults.ToList());
            return Median(allResults);
            // return allResults.First();
            // return allResults.Aggregate(BigInteger.Add);
        }
        
        async Task<IEnumerable<BigInteger>> ResultsOfIterationsAsync(SequenceContext context, int numberOfIterations, OperationFactory operationFactory)
        {                                
            SequenceGenerator generator = new SequenceGenerator(operationFactory); 
            var results = Enumerable.Range(1, numberOfIterations)
                .Select(entry => generator.GetRandomSequence(context.NumberOfOperations, context.InitialValue).BestPossibleResult);
            return results;           
        }
        
        BigInteger Median(IEnumerable<BigInteger> dataSet)
        {
            var calculator = new BigIntMedianCalculator(dataSet, _medianGroupsPerLevel, _medianLevels);
            return calculator.Calculate();
        }
    }    
}