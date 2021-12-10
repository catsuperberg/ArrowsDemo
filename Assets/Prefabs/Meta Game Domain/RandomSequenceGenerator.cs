using GamePlay;
using GameMeta.Operation;
using Sequence;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

using System.Linq;

namespace GameMeta
{
    public class RandomSequenceGenerator : IMetaGame
    {        
        float _coefficient = 0.7f;
        OperationPairsSequence _sequence = null;
        
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
            
            // show best sequence
            var testResult = new BigInteger(context.InitialValue);
            OperationExecutor exec = new OperationExecutor();
            Debug.Log("==================================================================");
            Debug.Log("==================================================================");
            Debug.Log("==================================================================");
            foreach(OperationPair pair in _sequence.Sequence)
            {
                Debug.Log("Pair: " + pair.LeftOperation.operationType.ToString() + " " + pair.LeftOperation.value + "  |||  " + pair.RightOperation.operationType.ToString() + " " + pair.RightOperation.value);
                Debug.Log("Best operation is: " + pair.BestOperation(testResult, exec).operationType.ToString() + " " + pair.BestOperation(testResult, exec).value);
                testResult = exec.Perform(pair.BestOperation(testResult, exec), testResult);
                Debug.Log("Result after operation = " + testResult);
            }
            
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
            var exec = new OperationExecutor();        
            OperationPairsSequence sequence;      
            OperationGenerator operationGenerator = new OperationGenerator();                 
            PairGenerator pairGenerator = new PairGenerator(0.5f, operationGenerator);
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
            return BigInteger.Divide(partialResult, new BigInteger(numberOfIterations));            
        }
        
        void GenerateSequenceUntilSuccesfull(CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, SequenceContext context)
        {
            float tempCoeff = _coefficient;
            var exec = new OperationExecutor();
            OperationGenerator operationGenerator = new OperationGenerator();
            PairGenerator pairGenerator = new PairGenerator(tempCoeff, operationGenerator); 
            SequenceGenerator generator = new SequenceGenerator(pairGenerator, exec);
            OperationPairsSequence sequence = new OperationPairsSequence(new List<OperationPair>{null});
            BigInteger result = new BigInteger(0);
            result += context.InitialValue;
            BigInteger spread = (new BigInteger(SpreadPercentage) * targetMaxResult)/new BigInteger(100);
            do
            {
                if(token.IsCancellationRequested)
                    return;
                sequence = generator.GetSequenceWithRandomPairs(context.NumberOfOperations, context.InitialValue);
                result = generator.CalculateBestResult(sequence.Sequence, context.InitialValue);             
            } while(BigInteger.Abs(targetMaxResult - result) >= spread);              
            
            if(_sequence == null)
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