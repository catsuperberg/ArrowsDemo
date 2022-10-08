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
        Container.Bind<IGameFolders>().To<GameFolders>().AsSingle();
        
        Container.Bind<OperationProbabilitiesFactory>().AsSingle();
        Container.Bind<MathOperationProbabilities>()
            .FromResolveGetter<OperationProbabilitiesFactory>(factory => factory.GetFromGeneratedJson());
        Container.BindFactory<OperationFactory, OperationFactory.Factory>().NonLazy();         
        
        Container.Bind<OperationExecutor>().AsTransient();
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
        
        var results = Enumerable.Range(1, 20).Select(entry => generator.GetAverageSequenceResult(context)).ToList();
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
        OperationPairsSequence sequence = null;
        
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
}
