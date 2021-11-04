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
        OperationExecutor _exec;   
        
        public RandomSequenceGenerator(OperationExecutor exec)
        {
            if(exec == null)
                    throw new System.Exception("OperationExecutor not provided to RandomSequenceGenerator");
            
            _exec = exec;
        }
        
        
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
            OperationPairsSequence sequence;      
            OperationGenerator operationGenerator = new OperationGenerator();                 
            PairGenerator pairGenerator = new PairGenerator(0.5f, operationGenerator, _exec);
            SequenceGenerator generator = new SequenceGenerator(pairGenerator, _exec); 
            BigInteger result = new BigInteger(0);
            for(int i = 0; i < numberOfIterations; i++)
            {
                sequence = generator.GetSequenceWithRandomPairs(context.NumberOfOperations);
                result = BigInteger.Add(result, generator.CalculateBestResult(sequence.Sequence, context.InitialValue));                 
            }
            return result;
        }
        
        void GenerateSequenceUntilSuccesfull(CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, SequenceContext context)
        {
            float tempCoeff = _coefficient;
            OperationGenerator operationGenerator = new OperationGenerator();
            PairGenerator pairGenerator; 
            SequenceGenerator generator;
            //HACK can't init OperationPairsSequence with empty list, token.IsCancellationRequested prevents sequnce to be initialized with guarantee
            OperationPairsSequence sequence = _sequence;
            BigInteger result = new BigInteger(context.InitialValue);
            BigInteger spread = BigInteger.Divide(BigInteger.Multiply(new BigInteger(SpreadPercentage), targetMaxResult), 100);
            do
            {
                if(token.IsCancellationRequested)
                    break;            
                if(result < targetMaxResult)
                    tempCoeff += 0.10f;
                else if(result > targetMaxResult)
                    tempCoeff -= 0.10f;
                pairGenerator = new PairGenerator(tempCoeff, operationGenerator, _exec);
                generator = new SequenceGenerator(pairGenerator, _exec);
                sequence = generator.GetSequenceWithRandomPairs(context.NumberOfOperations);
                result = generator.CalculateBestResult(sequence.Sequence, context.InitialValue);             
            } while(BigInteger.Abs(BigInteger.Subtract(targetMaxResult, result)) > spread);
            
            _sequence = sequence;
        }
    }    
}