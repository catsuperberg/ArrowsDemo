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
        const int _numIterationsForAverage = 40;  
        const int _generationTimout = 100;
        const int _exceptionTimout = 5000;
        
        static int _numThreads = Mathf.Clamp((int)((System.Environment.ProcessorCount/2)*0.75), 3, int.MaxValue);
        OperationFactory _prototypeFactory; // Beause cloning is faster than zenject
                
        RandomSequenceGenerator(OperationFactory.Factory operationFactory)
        {
            var opertionFactoryFactory = operationFactory ?? throw new System.ArgumentNullException(nameof(operationFactory)); 
            _prototypeFactory = opertionFactoryFactory.Create();       
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {        
            OperationPairsSequence? sequence = null;            
            var exceptionTimout = new System.Timers.Timer();
            exceptionTimout.Elapsed += (caller, e) => {throw new System.Exception($"Sequence generation stoped by timeout.\ntargetMaxResult was: {targetMaxResult}");};
            exceptionTimout.Interval = _exceptionTimout;
            exceptionTimout.Start();
            while (sequence == null)
                sequence = TryGeneratingSequence(targetMaxResult, SpreadPercentage, context);
            exceptionTimout.Stop();                      
            return (OperationPairsSequence)sequence;    
        }
        
        OperationPairsSequence? TryGeneratingSequence(BigInteger targetMaxResult, int SpreadPercentage,
            SequenceContext context)
        {            
            OperationPairsSequence? sequence = null;            
            while(sequence == null)
            {                
                CancellationTokenSource tokenSource= new CancellationTokenSource(); 
                CancellationToken ct = tokenSource.Token;                
                
                var generationTimout = new System.Timers.Timer();
                generationTimout.Elapsed += (caller, e) => {tokenSource.Cancel();};
                generationTimout.Interval = _generationTimout * context.NumberOfOperations;
                generationTimout.Start();
                
                var operationFactories = Enumerable.Range(1, _numThreads).Select(entry => _prototypeFactory.Clone());  
                var tasks = new List<Task<OperationPairsSequence?>>();
                foreach(var operationFactory in operationFactories)
                {
                    var task = Task.Run(() => {return GenerateSequenceAsync(ct, targetMaxResult, SpreadPercentage, context, operationFactory);});
                    tasks.Add(task);
                }
                Task.WaitAny(tasks.ToArray());            
                tokenSource.Cancel(); 
                generationTimout.Dispose();   
                sequence = tasks.FirstOrDefault(entry => entry.Result != null)?.Result;
            }
            return sequence;
        }
        
        async Task<OperationPairsSequence?> GenerateSequenceAsync(
            CancellationToken token, BigInteger targetMaxResult, int SpreadPercentage, 
            SequenceContext context, OperationFactory operationFactory)
        {
            OperationPairsSequence tempSequence;
            BigInteger result = new BigInteger(0);
            result += context.InitialValue;
            BigInteger spread = (new BigInteger(SpreadPercentage) * targetMaxResult)/new BigInteger(100);
            while(!token.IsCancellationRequested)
            {
                tempSequence = GetRandomSequence(context.NumberOfOperations, operationFactory, context.InitialValue);  
                result = tempSequence.BestPossibleResult;  
                if(BigInteger.Abs(targetMaxResult - result) < spread)
                    return tempSequence;
            }
            
            return null;
        }
        
        public BigInteger GetAverageSequenceResult(SequenceContext context)
            => GetAverageSequenceResult(context, _numIterationsForAverage);
        
        public BigInteger GetAverageSequenceResult(SequenceContext context, int numberOfIterations)
        {
            var iterationPerThread = numberOfIterations/_numThreads;
            var allResults = Enumerable.Range(0, _numThreads)
                .AsParallel()
                .Select(entry => ResultsOfIterations(context, iterationPerThread, _prototypeFactory.Clone()));
            return MathUtils.FastMedian(allResults.SelectMany(entry => entry).ToArray());
        }
        
        IEnumerable<BigInteger> ResultsOfIterations(SequenceContext context, int numberOfIterations, OperationFactory operationFactory)
        {                                
            var results = Enumerable.Range(1, numberOfIterations)
                .Select(entry => GetRandomSequence(context.NumberOfOperations, operationFactory, context.InitialValue).BestPossibleResult);
            return results;           
        }
        
                   
        public static OperationPairsSequence GetRandomSequence(int length, OperationFactory operationFactory, int initValue = 1)
        {                 
            var previousResult = new BigInteger(initValue);
            BigInteger tempResult;
            var sequence = operationFactory.GetInitialSequence(length);
            for(int i = 0; i < length; i++)
            {
                tempResult = sequence[i].BestResult(previousResult);
                while(tempResult <= 0)
                {
                    sequence[i] = operationFactory.GetRandomPair();
                    tempResult = sequence[i].BestResult(previousResult);
                }
                previousResult = tempResult;
            }
            
            return new OperationPairsSequence(sequence, previousResult);
        }    
    }    
}