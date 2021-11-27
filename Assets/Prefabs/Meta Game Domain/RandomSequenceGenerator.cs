using GamePlay;
using GameMeta.Operation;
using Sequence;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameMeta
{
    public class RandomSequenceGenerator : IMetaGame
    {        
        float _coefficient = 0.5f;
        OperationPairsSequence _sequence;
        
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {
            var numThreads = SystemInfo.processorCount - 1;
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
            var numThreads = SystemInfo.processorCount - 1;
            var iterationsPerThread = numberOfIterations/numThreads;            
            var results = new List<BigInteger>();  
            var resultSum = new BigInteger(0);            
            Thread[] threads = new Thread[numThreads];
            for(int thread = 0; thread < numThreads; thread++)
            {
                threads[thread] = new System.Threading.Thread(() => {
                    var result = SumOfRandomIterations(context, iterationsPerThread); results.Add(result); });
                threads[thread].Start();
                threads[thread].Join();
            }            
            foreach(BigInteger result in results)
                resultSum += result;            
            return BigInteger.Divide(resultSum, new BigInteger(numberOfIterations));
        }
        
        BigInteger SumOfRandomIterations(SequenceContext context, int numberOfIterations)
        {
            var exec = new OperationExecutor();
            OperationPairsSequence sequence;      
            OperationGenerator operationGenerator = new OperationGenerator();                 
            PairGenerator pairGenerator = new PairGenerator(0.5f, operationGenerator, exec);
            SequenceGenerator generator = new SequenceGenerator(pairGenerator, exec); 
            var partialResult = new BigInteger(0);
            var tempResult = new BigInteger(0);
            for(int i = 0; i < numberOfIterations; i++)
            {
                do
                {
                    sequence = generator.GetSequenceWithRandomPairs(context.NumberOfOperations);
                    tempResult = generator.CalculateBestResult(sequence.Sequence, context.InitialValue);             
                } while(tempResult == -1);
                partialResult = BigInteger.Add(partialResult, tempResult);     
            }
            return partialResult;            
        }
        
        void GenerateSequenceUntilSuccesfull(CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, SequenceContext context)
        {
            float tempCoeff = _coefficient;
            var exec = new OperationExecutor();
            OperationGenerator operationGenerator = new OperationGenerator();
            PairGenerator pairGenerator = new PairGenerator(tempCoeff, operationGenerator, exec); 
            SequenceGenerator generator = new SequenceGenerator(pairGenerator, exec);
            OperationPairsSequence sequence = new OperationPairsSequence(new List<OperationPair>{null});
            BigInteger result = new BigInteger(context.InitialValue);
            BigInteger spread = BigInteger.Divide(BigInteger.Multiply(new BigInteger(SpreadPercentage), targetMaxResult), 100);
            do
            {
                if(token.IsCancellationRequested)
                    break;
                sequence = generator.GetSequenceWithRandomPairs(context.NumberOfOperations);
                result = generator.CalculateBestResult(sequence.Sequence, context.InitialValue);             
            } while(BigInteger.Abs(BigInteger.Subtract(targetMaxResult, result)) >= spread);
            _sequence = sequence;
        }
        
        Task[] SpreadTaskToThreads(System.Action action, CancellationTokenSource tokenSource, int numThreads)
        { 
            Task[] threads = new Task[numThreads];        
            for(int thread = 0; thread < numThreads; thread++)
                threads[thread] = Task.Factory.StartNew(action, tokenSource.Token);
            return threads;
        }
    }    
}