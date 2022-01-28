using Game.Gameplay.Runtime.OperationSequence.Operation;
using GameMath;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Runtime.OperationSequence
{
    public class RandomSequenceGenerator : ISequenceCalculator
    {        
        float _coefficient = 0.7f;
        OperationPairsSequence _sequence = null;
        int _CPU_count = 1;
        
        RandomSequenceGenerator()
        {
            _CPU_count = SystemInfo.processorCount;
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {        
            _sequence = null;
            var numThreads = _CPU_count - 1;
            CancellationTokenSource tokenSource= new CancellationTokenSource(); 
            CancellationToken ct = tokenSource.Token;    
            var threads = SpreadTaskToThreads(() => GenerateSequenceUntilSuccesfull(ct, 
                    targetMaxResult, SpreadPercentage, context), tokenSource, numThreads);
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
            for(int thread = 0; thread < numThreads; thread++)
            {
                threads[thread] = new System.Threading.Thread(() => {
                    var result = SumOfRandomIterations(context, iterationsPerThread); results.Add(result); });
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
        
        BigInteger SumOfRandomIterations(SequenceContext context, int numberOfIterations)
        {             
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            var exec = new OperationExecutor();        
            OperationPairsSequence sequence;      
            SequenceGenerator generator = new SequenceGenerator(exec); 
            var partialResult = new BigInteger(0);
            var tempResult = new BigInteger(0);
            for(int i = 0; i < numberOfIterations; i++)
            {
                sequence = generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue);
                tempResult = sequence.ResultAtInitialGeneration;  
                partialResult = BigInteger.Add(partialResult, tempResult);     
            }
                           
            return BigInteger.Divide(partialResult, new BigInteger(numberOfIterations));            
        }
        
        void GenerateSequenceUntilSuccesfull(CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, SequenceContext context)
        {
            float tempCoeff = _coefficient;
            var exec = new OperationExecutor();
            SequenceGenerator generator = new SequenceGenerator(exec);
            OperationPairsSequence sequence = new OperationPairsSequence(new List<OperationPair>{null}, 0);
            BigInteger result = new BigInteger(0);
            result += context.InitialValue;
            BigInteger spread = (new BigInteger(SpreadPercentage) * targetMaxResult)/new BigInteger(100);
            do
            {
                if(token.IsCancellationRequested)
                    return;
                sequence = generator.GetSequenceWithRandomPairsOptimized(context.NumberOfOperations, context.InitialValue);  
                result = sequence.ResultAtInitialGeneration;             
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