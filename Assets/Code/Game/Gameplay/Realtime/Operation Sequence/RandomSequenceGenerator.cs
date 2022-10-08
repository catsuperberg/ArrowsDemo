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
        const int _numIterationsForAverage = 2000;  
        const int _generationTimout = 3000;
        
        float _coefficient = 0.7f;
        OperationPairsSequence _sequence = null;
        int _CPU_count = 1;
        OperationFactory.Factory _opertionFactory;
        
        RandomSequenceGenerator(OperationFactory.Factory operationFactory)
        {
            _CPU_count = SystemInfo.processorCount;
            _opertionFactory = operationFactory ?? throw new System.ArgumentNullException(nameof(operationFactory));
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {        
            _sequence = null;
            var numThreads = _CPU_count - 1;
            CancellationTokenSource tokenSource= new CancellationTokenSource(); 
            CancellationToken ct = tokenSource.Token;                
            
            var myTimer = new System.Timers.Timer();
            myTimer.Elapsed += (caller, e) => {tokenSource.Cancel();};
            myTimer.Interval = _generationTimout;
            myTimer.Start();
            
            var operationFactories = Enumerable.Range(1, numThreads).Select(entry => _opertionFactory.Create());
            var tasks = new List<Task<OperationPairsSequence>>();
            foreach(var operationFactory in operationFactories)
            {
                var task = Task.Run(() => {return GenerateSequenceAsync(ct, targetMaxResult, SpreadPercentage, context, operationFactory);});
                tasks.Add(task);
            }
            Task.WaitAny(tasks.ToArray());            
            tokenSource.Cancel(); 
            myTimer.Dispose();   
            _sequence = tasks.FirstOrDefault(entry => entry.Result != null).Result;
               
            if(_sequence == null)
                throw new System.Exception($"No sequence generated but trying to return.\n targetMaxResult was: {targetMaxResult}");         
            return _sequence;    
        }
        
        public BigInteger GetAverageSequenceResult(SequenceContext context)
            => GetAverageSequenceResult(context, _numIterationsForAverage);
        
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations)
        {
            var numThreads = 20;
            var medianCount = 4;
            var iterationsPerThread = numberOfIterations/numThreads;           
            var forMedian = new List<BigInteger>();      
            var operationFactories = Enumerable.Range(1, numThreads).Select(entry => _opertionFactory.Create());
            var tasks = new List<Task<BigInteger>>();
            
            // var factorieGroups = new List<List<OperationFactory>>();
            // foreach(var i in Enumerable.Range(1, numThreads))
            //     factorieGroups.Add(Enumerable.Range(1, iterationsPerThread).Select(entry => _opertionFactory.Create()).ToList());
            // var tasks = new List<Task<BigInteger>>();
            // foreach(var factories in factorieGroups)
            // {
            //     var task = Task.Run(() => {return MeanOfIterationsAsync(context, iterationsPerThread, factories);});
            //     tasks.Add(task);
            // }
            
            foreach(var operationFactory in operationFactories)
            {
                var task = Task.Run(() => {return MeanOfIterationsAsync(context, iterationsPerThread, operationFactory);});
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
            var results = tasks.Select(entry => entry.Result).ToList();  
            var medianEveryN = numThreads / medianCount;   
            var tempResult = new BigInteger(0);       
            var count = 1;      
            foreach(BigInteger result in results)
            {
                tempResult += result;
                if(count >= medianEveryN)
                {         
                    count = 1;
                    var medianResult = tempResult/medianCount;
                    forMedian.Add(medianResult);     
                    tempResult = new BigInteger(0);    
                }
                else               
                    count ++; 
            }
            return MathUtils.Median<BigInteger>(forMedian);
        }
                
        // async Task<BigInteger> MeanOfIterationsAsync(SequenceContext context, int numberOfIterations, IEnumerable<OperationFactory> operationFactories)
        async Task<BigInteger> MeanOfIterationsAsync(SequenceContext context, int numberOfIterations, OperationFactory operationFactory)
        {                         
            // var execs = new Queue<OperationExecutor>(Enumerable.Range(1, numberOfIterations).Select(entry => new OperationExecutor()));
            // var generators = new Queue<SequenceGenerator>(Enumerable.Range(1, numberOfIterations).Select(entry => new SequenceGenerator(execs.Dequeue(), operationFactory)));
            
            // var tasks = Enumerable.Range(1, numberOfIterations)
            //     .Select(entry => GenerateSequence(generators.Dequeue(), context, operationFactory))
            //     .ToArray();
                
            // // var tasks = Enumerable.Range(1, numberOfIterations)
            // //     .Select(entry => GenerateSequence(context, operationFactory))
            // //     .ToArray();
            
            // Task.WaitAll(tasks);
            // var results = tasks.Select(entry => entry.Result).ToList(); 
            
            // var exec = new OperationExecutor();        
            // SequenceGenerator generator = new SequenceGenerator(exec, operationFactory); 
            // var results = generator.GetSequences(context.NumberOfOperations, numberOfIterations, context.InitialValue).Select(entry => entry.BestPossibleResult);
            
            var exec = new OperationExecutor();        
            SequenceGenerator generator = new SequenceGenerator(exec, operationFactory); 
            var results = Enumerable.Range(1, numberOfIterations)
                .Select(entry => generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue).BestPossibleResult);
            
            var sumOfResults = results.Aggregate(BigInteger.Add);                
            return BigInteger.Divide(sumOfResults, new BigInteger(numberOfIterations));            
        }
        
        
        // async Task<BigInteger> GenerateSequence(SequenceGenerator generator, SequenceContext context, OperationFactory operationFactory)
        // {
        //     return generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue).BestPossibleResult;
        // }
        
        // async Task<BigInteger> GenerateSequence(SequenceContext context, OperationFactory operationFactory)
        // {
        //     var exec = new OperationExecutor();        
        //     SequenceGenerator generator = new SequenceGenerator(exec, operationFactory); 
        //     return generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue).BestPossibleResult;
        // }
        
        async Task<OperationPairsSequence> GenerateSequenceAsync(
            CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, 
            SequenceContext context, OperationFactory operationFactor)
        {
            float tempCoeff = _coefficient;
            var exec = new OperationExecutor();
            var generator = new SequenceGenerator(exec, operationFactor);
            OperationPairsSequence tempSequence;
            BigInteger result = new BigInteger(0);
            result += context.InitialValue;
            BigInteger spread = (new BigInteger(SpreadPercentage) * targetMaxResult)/new BigInteger(100);
            while(!token.IsCancellationRequested)
            {
                tempSequence = generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue);  
                result = tempSequence.BestPossibleResult;  
                if(BigInteger.Abs(targetMaxResult - result) < spread)
                    return tempSequence;
            }
            
            // do
            // {
            //     if(token.IsCancellationRequested)
            //         break;
            //     tempSequence = generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue);  
            //     result = tempSequence.BestPossibleResult;             
            // } while(BigInteger.Abs(targetMaxResult - result) >= spread);    
            
            return null;
        }
    }    
}