using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using UnityEngine;
using Utils;

using Timer = System.Timers.Timer;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class RandomSequenceGenerator : ISequenceCalculator
    {       
        const int _numIterationsForAverage = 40;  
        const int _generationTimout = 80;
        
        static int _numThreads = Mathf.Clamp((int)((System.Environment.ProcessorCount/2)-1), 3, int.MaxValue);
        System.Random _rand = new System.Random(Guid.NewGuid().GetHashCode());
        OperationFactory _prototypeFactory; // Beause cloning is faster than zenject
        IEnumerable<OperationFactory> _operationFactories;
        IOperationRules _operationRules;
        static IEnumerable<int> _threadRange = Enumerable.Range(0, _numThreads);
        static IEnumerable<int> _iterationsRange = Enumerable.Range(0, _numIterationsForAverage);
        IEnumerable<OperationPairsSequence> _sortedSequencesWithResults = null;
        int _medianAt = -1;
                
        RandomSequenceGenerator(OperationFactory.Factory operationFactory)
        {
            var opertionFactoryFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory)); 
            _prototypeFactory = opertionFactoryFactory.Create();   
            _operationRules = _prototypeFactory.OperationRules;    
            _operationFactories = _iterationsRange
                .AsParallel()
                .Select(entry => OperationFactory.FullConstructor(_operationRules))
                .ToList();
        }
        
        public OperationPairsSequence GetSequenceInSpreadRange(BigInteger targetResult, int spreadPercents,
            SequenceContext context)
        {            
            BigInteger spread = (new BigInteger(spreadPercents) * targetResult)/new BigInteger(100);     
            var range = new NumberRange<BigInteger>(targetResult-spread, targetResult+spread);
            return GetSequenceFromGenerated(targetResult, range) ?? GenerateUntilSucessful(targetResult, range, context);
        }
        
        OperationPairsSequence GetSequenceFromGenerated(BigInteger targetResult, NumberRange<BigInteger> limits)
        {        
            if(_sortedSequencesWithResults == null)
                return null;
            
            var centerResult = _sortedSequencesWithResults.ElementAt(_medianAt).BestPossibleResult;
            if(!limits.Fits(centerResult))
                return null;
                
            var sequencesInRange = _sortedSequencesWithResults.Where(entry => limits.Fits(entry.BestPossibleResult));            
            var selectionAt = _rand.Next(0, sequencesInRange.Count());
            return sequencesInRange.ElementAt(selectionAt);
        }
        
        OperationPairsSequence GenerateUntilSucessful(BigInteger targetMaxResult, NumberRange<BigInteger> limits, SequenceContext context)
        {            
            OperationPairsSequence sequence = null;       
            var tryCount = 50;
            while (sequence == null)
            {
                if(tryCount-- <= 0 ) throw new System.Exception($"Sequence generation stoped by try limit.\ntargetMaxResult was: {targetMaxResult}, range : {limits.Min}...{limits.Max}");
                sequence = TryGeneratingSequence(limits, context);
            }                    
            return sequence;    
        }
        
        // void WaitForThreads()
        // {
        //     int worker,io = 0;
        //     int workerMax,ioMax = 0;
        //     ThreadPool.GetMaxThreads(out workerMax, out ioMax);
        //     ThreadPool.GetAvailableThreads(out worker, out io);
        //     var threadsUsed = workerMax - worker;
        //     while(threadsUsed >= System.Environment.ProcessorCount*0.7)
        //     {
        //         Thread.Sleep(2);
        //         ThreadPool.GetAvailableThreads(out worker, out io);
        //         threadsUsed = workerMax - worker;     
        //         Debug.Log($"threads used: {threadsUsed}");         
        //     }
        // }
        
        OperationPairsSequence TryGeneratingSequence(NumberRange<BigInteger> limits, SequenceContext context)
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
                .Select(factory => GenerateSequence(ct, tokenSource, limits, context, factory))
                .ToList();
            tokenSource.Cancel(); 
            generationTimout.Dispose();   
            return allSequences.FirstOrDefault(entry => entry != null);
        }
        
        static OperationPairsSequence GenerateSequence(
            CancellationToken token, CancellationTokenSource tokenSource, NumberRange<BigInteger> limits, 
            SequenceContext context, OperationFactory operationFactory)
        {
            OperationPairsSequence tempSequence;
            BigInteger result = BigInteger.Zero;
            result += context.InitialValue;
            while(!token.IsCancellationRequested)
            {
                tempSequence = GenerateRandomSequence(context.NumberOfOperations, operationFactory, context.InitialValue);  
                result = tempSequence.CalculateResult();  
                if(limits.Fits(result))
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
            var sequences = _operationFactories
                .AsParallel()
                .WithDegreeOfParallelism(_numThreads)
                .Select(factory => 
                    {
                        var sequence = GenerateRandomSequence(context.NumberOfOperations, factory, context.InitialValue); 
                        sequence.CalculateResult(); 
                        return sequence;
                    })
                .ToList();
            
            _sortedSequencesWithResults = sequences
                .OrderBy(entry => entry.BestPossibleResult);
            
            _medianAt = MathUtils.FastMedianIndexPreSorted(_sortedSequencesWithResults);
            return _sortedSequencesWithResults.ElementAt(_medianAt).BestPossibleResult;
        }
                
        
        // public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations)
        // {
        //     var iterationsPerThread = numberOfIterations/_numThreads;
            
        //     var iterationRange = Enumerable.Range(0, iterationsPerThread);
        //     var sequencesWithResults = _threadRange
        //         .AsParallel()
        //         .WithDegreeOfParallelism(_numThreads)
        //         .Select(entry => OperationFactory.FullConstructor(_operationRules))
        //         .Select(factory => ResultsOfIterations(context, iterationRange, factory))
        //         .SelectMany(entry => entry)
        //         .Select(sequence => (result: sequence.BestPossibleResult(), sequence: sequence))
        //         .ToList();
                
        //     _sortedSequencesWithResults = sequencesWithResults
        //         .OrderBy(entry => entry.result);
            
        //     _medianAt = MathUtils.FastMedianIndexPreSorted(_sortedSequencesWithResults.Select(entry => entry.result).ToList());
        //     return _sortedSequencesWithResults.ElementAt(_medianAt).result;
        // }
        
        static IEnumerable<OperationPairsSequence> ResultsOfIterations(SequenceContext context, IEnumerable<int> iterationRange, OperationFactory operationFactory)
        {                                
            var results = iterationRange
                .Select(entry => GenerateRandomSequence(context.NumberOfOperations, operationFactory, context.InitialValue));
            return results;           
        }    
        
        public OperationPairsSequence GetRandomSequence(SequenceContext context)    
        {
            var opFactory = OperationFactory.FullConstructor(_operationRules);
            var sequence = opFactory.GetInitialSequence(context.NumberOfOperations);
            FastLessThanOneReplacement(sequence, opFactory, context.InitialValue);
            return new OperationPairsSequence(sequence, context.InitialValue);
        }
                   
        static OperationPairsSequence GenerateRandomSequence(int length, OperationFactory operationFactory, int initValue = 1)
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
                var tempResult = sequence[i].BestOperation(previousResult).Type.ApplyMiniMath(previousResult);
                if(tempResult < 1)
                    sequence[i] = operationFactory.GetPositivePair();
                else
                    previousResult = tempResult;
            }
        }
    }    
}