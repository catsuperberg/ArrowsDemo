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
            var operationFactories = new Queue<OperationFactory>(Enumerable.Repeat(_opertionFactory.Create(), numThreads));
            var threads = SpreadTaskToThreads(() => GenerateSequenceUntilSuccesfull(ct, 
                    targetMaxResult, SpreadPercentage, context, operationFactories.Dequeue()), tokenSource, numThreads);
            Task.WaitAny(threads); 
            tokenSource.Cancel();              
            return _sequence;    
        }
        
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations)
        {
            var numThreads = 20;
            var medianCount = 4;
            var iterationsPerThread = numberOfIterations/numThreads;            
            var results = new List<BigInteger>();           
            var forMedian = new List<BigInteger>();      
            Thread[] threads = new Thread[numThreads];
            var operationFactories = new Queue<OperationFactory>(Enumerable.Repeat(_opertionFactory.Create(), numThreads));
            for(int thread = 0; thread < numThreads; thread++)
            {
                threads[thread] = new System.Threading.Thread(() => {
                    var result = SumOfRandomIterations(context, iterationsPerThread, operationFactories.Dequeue()); results.Add(result); });
                threads[thread].Start();
                threads[thread].Join();
            }            
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
        
        BigInteger SumOfRandomIterations(SequenceContext context, int numberOfIterations, OperationFactory operationFactory)
        {             
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            var exec = new OperationExecutor();        
            OperationPairsSequence sequence;      
            SequenceGenerator generator = new SequenceGenerator(exec, operationFactory); 
            var partialResult = new BigInteger(0);
            var tempResult = new BigInteger(0);
            for(int i = 0; i < numberOfIterations; i++)
            {
                sequence = generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue);
                tempResult = sequence.ResultAtCreation;  
                partialResult = BigInteger.Add(partialResult, tempResult);     
            }
                           
            return BigInteger.Divide(partialResult, new BigInteger(numberOfIterations));            
        }
        
        void GenerateSequenceUntilSuccesfull(
            CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, 
            SequenceContext context, OperationFactory operationFactor)
        {
            float tempCoeff = _coefficient;
            var exec = new OperationExecutor();
            SequenceGenerator generator = new SequenceGenerator(exec, operationFactor);
            OperationPairsSequence sequence = new OperationPairsSequence(new List<OperationPair>{null}, 0);
            BigInteger result = new BigInteger(0);
            result += context.InitialValue;
            BigInteger spread = (new BigInteger(SpreadPercentage) * targetMaxResult)/new BigInteger(100);
            do
            {
                if(token.IsCancellationRequested)
                    return;
                sequence = generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue);  
                result = sequence.ResultAtCreation;             
            } while(BigInteger.Abs(targetMaxResult - result) >= spread);              
            
            if(_sequence == null)
                _sequence = sequence;   
        }
        
        Task[] SpreadTaskToThreads(System.Action action, CancellationTokenSource tokenSource, int numThreads)
        { 
            Task[] threads = new Task[numThreads];        
            for(int thread = 0; thread < numThreads; thread++)
                threads[thread] = Task.Run(action, tokenSource.Token);
            return threads;
        }
    }    
}