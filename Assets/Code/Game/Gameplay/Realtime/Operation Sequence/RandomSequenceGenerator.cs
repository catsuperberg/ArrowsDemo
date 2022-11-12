using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Timer = System.Timers.Timer;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class RandomSequenceGenerator : ISequenceCalculator
    {       
        const int _numIterationsForAverage = 40;  
        const int _generationTimout = 80;
        const int _exceptionTimout = 2000;
        
        static int _numThreads = Mathf.Clamp((int)((System.Environment.ProcessorCount/2)-1), 3, int.MaxValue);
        OperationFactory _prototypeFactory; // Beause cloning is faster than zenject
        IOperationRules _operationRules;
        IEnumerable<int> _threadRange = Enumerable.Range(0, _numThreads);
                
        RandomSequenceGenerator(OperationFactory.Factory operationFactory)
        {
            var opertionFactoryFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory)); 
            _prototypeFactory = opertionFactoryFactory.Create();   
            _operationRules = _prototypeFactory.OperationRules;    
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {        
            OperationPairsSequence? sequence = null;            
            var exceptionTimout = new Timer();
            var throwException = false;
            
            exceptionTimout.Elapsed += (caller, args) => {throwException = true;};
            exceptionTimout.Interval = _exceptionTimout;
            exceptionTimout.Start();
            while (sequence == null)
            {
                sequence = TryGeneratingSequence(targetMaxResult, SpreadPercentage, context);
                if(throwException) throw new System.Exception($"Sequence generation stoped by timeout.\ntargetMaxResult was: {targetMaxResult}");
            }
            exceptionTimout.Stop();                      
            return (OperationPairsSequence)sequence;    
        }
        
        OperationPairsSequence? TryGeneratingSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {                
            CancellationTokenSource tokenSource= new CancellationTokenSource(); 
            CancellationToken ct = tokenSource.Token;           
            
            var generationTimout = new Timer();
            generationTimout.Elapsed += (caller, e) => {tokenSource.Cancel();};
            generationTimout.Interval = _generationTimout;
            generationTimout.Start();       
            
            var allSequences = _threadRange
                .AsParallel()
                .WithDegreeOfParallelism(_numThreads)
                .Select(entry => OperationFactory.FullConstructor(_operationRules))
                .Select(factory => GenerateSequence(ct, tokenSource, targetMaxResult, SpreadPercentage, context, factory))
                .ToList();
            tokenSource.Cancel(); 
            generationTimout.Dispose();   
            return allSequences.FirstOrDefault(entry => entry != null);
        }
        
        OperationPairsSequence? GenerateSequence(
            CancellationToken token, CancellationTokenSource tokenSource, BigInteger targetMaxResult, 
            int SpreadPercentage, SequenceContext context, OperationFactory operationFactory)
        {
            OperationPairsSequence tempSequence;
            BigInteger result = BigInteger.Zero;
            result += context.InitialValue;
            BigInteger spread = (new BigInteger(SpreadPercentage) * targetMaxResult)/new BigInteger(100);
            while(!token.IsCancellationRequested)
            {
                tempSequence = GetRandomSequence(context.NumberOfOperations, operationFactory, context.InitialValue);  
                result = tempSequence.BestPossibleResult();  
                if(BigInteger.Abs(targetMaxResult - result) < spread)
                {                    
                    tokenSource.Cancel();
                    return tempSequence;
                }
            }
            return null;
        }
        
        public BigInteger GetAverageSequenceResult(SequenceContext context)
            => GetAverageSequenceResult(context, _numIterationsForAverage);
        
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations)
        {
            var iterationsPerThread = numberOfIterations/_numThreads;
            
            var iterationRange = Enumerable.Range(0, iterationsPerThread);
            var results = _threadRange
                .AsParallel()
                .WithDegreeOfParallelism(_numThreads)
                .Select(entry => OperationFactory.FullConstructor(_operationRules))
                .Select(factory => ResultsOfIterations(context, iterationRange, factory))
                .SelectMany(entry => entry)
                .Select(sequence => sequence.BestPossibleResult())
                .ToList();
            
            return MathUtils.FastMedian(results);
        }
        
        static IEnumerable<OperationPairsSequence> ResultsOfIterations(SequenceContext context, IEnumerable<int> iterationRange, OperationFactory operationFactory)
        {                                
            var results = iterationRange
                .Select(entry => GetRandomSequence(context.NumberOfOperations, operationFactory, context.InitialValue));
            return results;           
        }        
                   
        public static OperationPairsSequence GetRandomSequence(int length, OperationFactory operationFactory, int initValue = 1)
        {                 
            var sequence = operationFactory.GetInitialSequence(length);
            FastLessThanOneReplacement(sequence, operationFactory, initValue);
            return new OperationPairsSequence(sequence, initValue);
        }    
        
        static void FastLessThanOneReplacement(OperationPair[] sequence, OperationFactory operationFactory, int initialValue)
        {
            var previousResult = initialValue;
            for(int i = 0; i < sequence.Count(); i++)
            {
                var tempResult = sequence[i].BestOperation(initialValue).Type.ApplyMiniMath(previousResult);
                if(tempResult <= 0)
                    sequence[i] = operationFactory.GetPositivePair();
                else
                    previousResult = tempResult;
            }
        }
    }    
}