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
            CancellationTokenSource tokenSource= new CancellationTokenSource(); 
            CancellationToken ct = tokenSource.Token; 
            int numThreads = SystemInfo.processorCount - 1;
            Task[] threads = new Task[numThreads];        
            for(int thread = 0; thread < numThreads; thread++)
            {
                threads[thread] = Task.Factory.StartNew(() => GenerateSequenceUntilSuccesfull(ct, 
                    targetMaxResult, SpreadPercentage, context), tokenSource.Token);
            }
            Task.WaitAny(threads); 
            tokenSource.Cancel();  
            return _sequence;    
        }
        
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations)
        {
            var exec = new OperationExecutor();
            OperationPairsSequence sequence;      
            OperationGenerator operationGenerator = new OperationGenerator();                 
            PairGenerator pairGenerator = new PairGenerator(0.5f, operationGenerator, exec);
            SequenceGenerator generator = new SequenceGenerator(pairGenerator, exec); 
            BigInteger result = new BigInteger(0);
            BigInteger resultSum = new BigInteger(0);
            BigInteger tempResult = new BigInteger(0);
            for(int i = 0; i < numberOfIterations; i++)
            {
                do
                {
                    sequence = generator.GetSequenceWithRandomPairs(context.NumberOfOperations);
                    tempResult = generator.CalculateBestResult(sequence.Sequence, context.InitialValue);             
                } while(tempResult == -1);
                resultSum = BigInteger.Add(resultSum, tempResult);     
            }
            return BigInteger.Divide(resultSum, new BigInteger(numberOfIterations));
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
                sequence = generator.GetSequenceWithRandomPairs(context.NumberOfOperations);
                result = generator.CalculateBestResult(sequence.Sequence, context.InitialValue);             
            } while(BigInteger.Abs(BigInteger.Subtract(targetMaxResult, result)) >= spread);
            _sequence = sequence;
        }
    }    
}