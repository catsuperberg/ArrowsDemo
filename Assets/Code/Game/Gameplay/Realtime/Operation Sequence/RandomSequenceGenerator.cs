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
        const int _numIterationsForAverage = 32;  
        const int _generationTimout = 3000;
        
        int _CPU_count = 1;
        int _numThreads = 1;
        OperationFactory.Factory _opertionFactoryFactory;
        OperationFactory _prototypeFactory;
        
        RandomSequenceGenerator(OperationFactory.Factory operationFactory)
        {
            _CPU_count = SystemInfo.processorCount;
            _numThreads = Mathf.Clamp((_CPU_count * 2) -1, 1, int.MaxValue);
            _opertionFactoryFactory = operationFactory ?? throw new System.ArgumentNullException(nameof(operationFactory)); 
            _prototypeFactory = _opertionFactoryFactory.Create();       
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {        
            CancellationTokenSource tokenSource= new CancellationTokenSource(); 
            CancellationToken ct = tokenSource.Token;                
            
            var generationTimout = new System.Timers.Timer();
            generationTimout.Elapsed += (caller, e) => {tokenSource.Cancel();};
            generationTimout.Interval = _generationTimout;
            generationTimout.Start();
            
            var operationFactories = Enumerable.Range(1, _numThreads).Select(entry => _prototypeFactory.Clone(context.NumberOfOperations));  
            var tasks = new List<Task<OperationPairsSequence>>();
            foreach(var operationFactory in operationFactories)
            {
                var task = Task.Run(() => {return GenerateSequenceAsync(ct, targetMaxResult, SpreadPercentage, context, operationFactory);});
                tasks.Add(task);
            }
            Task.WaitAny(tasks.ToArray());            
            tokenSource.Cancel(); 
            generationTimout.Dispose();   
            var sequence = tasks.FirstOrDefault(entry => entry.Result != default(OperationPairsSequence)).Result;
               
            if(sequence == default(OperationPairsSequence))
                throw new System.Exception($"No sequence generated but trying to return.\n targetMaxResult was: {targetMaxResult}");         
            return sequence;    
        }
        
        async Task<OperationPairsSequence> GenerateSequenceAsync(
            CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, 
            SequenceContext context, OperationFactory operationFactory)
        {
            OperationPairsSequence tempSequence;
            BigInteger result = new BigInteger(0);
            result += context.InitialValue;
            BigInteger spread = (new BigInteger(SpreadPercentage) * targetMaxResult)/new BigInteger(100);
            while(!token.IsCancellationRequested)
            {
                tempSequence = SequenceGenerator.GetRandomSequence(context.NumberOfOperations, operationFactory, context.InitialValue);  
                result = tempSequence.BestPossibleResult;  
                if(BigInteger.Abs(targetMaxResult - result) < spread)
                    return tempSequence;
            }
            
            return default(OperationPairsSequence);
        }
        
        public BigInteger GetAverageSequenceResult(SequenceContext context)
            => GetAverageSequenceResult(context, _numIterationsForAverage);
        
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations)
        {
            var iterationPerThread = numberOfIterations/_numThreads;
            var allResults = Enumerable.Range(0, _numThreads)
                .AsParallel()
                .Select(entry => ResultsOfIterations(context, iterationPerThread, _prototypeFactory.Clone(context.NumberOfOperations)));
            return MathUtils.FastMedian(allResults.SelectMany(entry => entry).ToArray());
        }
        
        IEnumerable<BigInteger> ResultsOfIterations(SequenceContext context, int numberOfIterations, OperationFactory operationFactory)
        {                                
            var results = Enumerable.Range(1, numberOfIterations)
                .Select(entry => SequenceGenerator.GetRandomSequence(context.NumberOfOperations, operationFactory, context.InitialValue).BestPossibleResult);
            return results;           
        }
    }    
}