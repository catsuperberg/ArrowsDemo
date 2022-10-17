using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using ExtensionMethods;
using Game.GameDesign;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using GameDesign;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

using Debug = UnityEngine.Debug;

public class SequenceGenerationTests : ZenjectUnitTestFixture
{
    [SetUp]
    public void TestSetup()
    {
        ComposeSequence();
    }
        
    void ComposeSequence()
    {        
        var folders = new GameFolders();
        // Container.Bind<IGameFolders>().FromInstance(folders);
        var probabilitiesFactory = new OperationProbabilitiesFactory(folders);
        
        // Container.Bind<OperationProbabilitiesFactory>().AsTransient();
        Container.Bind<OperationProbabilitiesFactory>().FromInstance(probabilitiesFactory).AsTransient();
        // Container.Bind<MathOperationProbabilities>()
        //     .FromResolveGetter<OperationProbabilitiesFactory>(factory => factory.GetFromGeneratedJson()).AsSingle();
        Container.BindFactory<OperationFactory, OperationFactory.Factory>().NonLazy();         
        
        Container.Bind<IOperationRules>().To<OperationRules>().AsTransient();
        Container.Bind<ISequenceCalculator>().To<RandomSequenceGenerator>().AsSingle();
        Container.Bind<ISequenceManager>().To<SequenceManager>().AsSingle();    
    }
    
    [Test, Performance, RequiresPlayMode(false)]
    public void TestLateAverageGenerationPerformance()
    {        
        var context = new SequenceContext(500, 3, 50, 8);
        var generator = Container.Resolve<ISequenceCalculator>();
        
        Measure.Method(() => {generator.GetAverageSequenceResult(context);})
            .WarmupCount(2)
            .MeasurementCount(50)
            .Run();
    }
    
    [Test, Performance, RequiresPlayMode(false)]
    public void TestLateAverageGenerationPerformanceShort()
    {        
        var context = new SequenceContext(500, 3, 50, 8);
        var generator = Container.Resolve<ISequenceCalculator>();
        
        Measure.Method(() => {generator.GetAverageSequenceResult(context);})
            .WarmupCount(1)
            .MeasurementCount(3)
            .Run();
    }
    
    [Test, Performance, RequiresPlayMode(false)]
    public void TestEarlyAverageGenerationPerformance()
    {        
        var context = new SequenceContext(500, 3, 8, 8);
        var generator = Container.Resolve<ISequenceCalculator>();
        
        Measure.Method(() => {generator.GetAverageSequenceResult(context);})
            .WarmupCount(2)
            .MeasurementCount(50)
            .Run();
    }
        
    [Test, RequiresPlayMode(false)]
    public void AverageGenerationOutput()
    {        
        var context = new SequenceContext(500, 3, 50, 8);
        var generator = Container.Resolve<ISequenceCalculator>();
        
        var results = Enumerable.Range(1, 50).Select(entry => generator.GetAverageSequenceResult(context)).ToList();
        results.ForEach(entry => Debug.Log(entry.ParseToReadable()));
    }
    
    
    [Test, RequiresPlayMode(false)]
    public void GeneratingSequenceForTargetIsPossilbe()
    {           
        var repeats = 20;
        var context = new SequenceContext(500, 3, 50, 8);
        var generator = Container.Resolve<ISequenceCalculator>();        
        var averageTarget = generator.GetAverageSequenceResult(context);  
        Debug.Log($"Target is: {averageTarget}");
        Enumerable.Range(1,repeats).ToList().ForEach(entry => GenerateSequnce(averageTarget, generator, context));
    }    
    
    
    void GenerateSequnce(BigInteger targer, ISequenceCalculator generator, SequenceContext context)
    {
        var spread = 15;
        OperationPairsSequence sequence;
        
        try
        {
            sequence = generator.GenerateSequence(targer, spread, context);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Coulnd't generate sequence");
            Debug.Log(ex);
        }
                
        Assert.That(sequence, Is.Not.Null);
        
        var absoluteSpread = BigInteger.Abs(targer - sequence.BestPossibleResult);
        var percentSpread = BigInteger.Multiply(absoluteSpread, new BigInteger(100))/targer;
        Debug.Log($"Sequence result: {sequence.BestPossibleResult.ParseToReadable()} Abs spread: {absoluteSpread.ParseToReadable()} Spread percents: {percentSpread}");
    }
    
    
    [Test, RequiresPlayMode(false)]
    public void FastBestSameAsSlow()
    {           
        var repeats = 500;
        var context = new SequenceContext(500, 3, 50, 8);
        var generator = Container.Resolve<ISequenceCalculator>();        
        var averageTarget = generator.GetAverageSequenceResult(context);  
        Debug.Log($"Target is: {averageTarget}");
        var sequences = Enumerable.Range(1,repeats).Select(entry => generator.GenerateSequence(averageTarget, 15, context));
        foreach(var sequence in sequences)
        {
            var seq = sequence.Sequence;
            var fastResult = new BigInteger(context.InitialValue);
            var fullResult = new BigInteger(context.InitialValue);
            var previous = new BigInteger(context.InitialValue);
            for(int i = 0; i < context.NumberOfOperations; i++)
            {              
                var fastBest = seq.ElementAt(i).BestOperation(fastResult);
                var fullBest = seq.ElementAt(i).FullBestOperation(fullResult);
                
                fastResult = fastBest.Perform(fastResult);
                fullResult = fullBest.Perform(fullResult);
                if(fastResult != fullResult)
                {
                    Debug.Log($"previous is: {previous}");
                    Debug.Log($"Fast result is: {fastResult}");
                    Debug.Log($"Full result is: {fullResult}");
                    Debug.Log($"Left: {seq.ElementAt(i).LeftOperation.Type.ToString()} {seq.ElementAt(i).LeftOperation.Value} | Right:  {seq.ElementAt(i).RightOperation.Type.ToString()} {seq.ElementAt(i).RightOperation.Value}");
                    Debug.Log($"Fast best: {fastBest.Type.ToString()} {fastBest.Value}");
                    Debug.Log($"Full best: {fullBest.Type.ToString()} {fullBest.Value}");
                    Debug.Log($"========================================");
                    Assert.That(fastResult, Is.EqualTo(fullResult));
                }
                previous = fastResult;
            }
        }
    }    
    
    [Test, RequiresPlayMode(false)]
    public void TestFastLeftIsBest()
    {           
        var rules = new OperationRules();
        var pairsDifferentTypes = new List<OperationPair>();
        var value = 2;
        var types = Enumerable.Range((int)Operation.First, (int)Operation.Last);
        foreach(var type in types)
            foreach(var secondType in types.Where(entry => entry != type))
                pairsDifferentTypes.Add(new OperationPair(new OperationInstance((Operation)type, value, rules), new OperationInstance((Operation)secondType, value, rules), rules));
        
        var pairsSameTypes = new List<OperationPair>();   
        var less = 2;
        var more = 3;     
        foreach(var type in types)
        {
            pairsDifferentTypes.Add(new OperationPair(new OperationInstance((Operation)type, more, rules), new OperationInstance((Operation)type, less, rules), rules));
            pairsDifferentTypes.Add(new OperationPair(new OperationInstance((Operation)type, less, rules), new OperationInstance((Operation)type, more, rules), rules));
        }
    }   
}
