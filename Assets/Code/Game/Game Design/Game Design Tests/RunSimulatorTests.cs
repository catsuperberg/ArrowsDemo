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
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

using Debug = UnityEngine.Debug;


[TestFixture]
public class RunSimulatorTests : ZenjectUnitTestFixture
{
    // [Inject]
    RunSimulator _simulator;
    
    [SetUp]
    public void TestSetup()
    {
        ComposeSequence();
        ComposeTargetGenerator();
        ComposePlayer();
        // Container.Inject(this);
        Container.Bind<RunSimulator>().AsTransient();
        _simulator = Container.Resolve<RunSimulator>();
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
    
    void ComposeTargetGenerator()
    {
        Container.Bind<ITargetProvider>().To<TargetDataOnlyGenerator>().AsTransient();
    }
    
    void ComposePlayer()
    {
        var executor = new OperationExecutor();
        var gateSelector = new GateSelector(GateSelectors.BadPlayer.Chance(), executor);
        var adSelector = new AdSkipper();
        var player = new VirtualPlayer(gateSelector, adSelector, executor);
        Container.Bind<VirtualPlayer>().FromInstance(player).AsTransient(); 
    }
    
    [Test, Performance]
    public void SimulationPerformanceTest()
    {
        Measure.Method(RunSingleSimulation)
            .MeasurementCount(5)
            .Run();
    }
    
    
    [Test]
    public void SimulateWithResultsOutput()
    {
        RunSingleSimulation();
    }
    
    [Test]
    [RequiresPlayMode(false)]
    public void RunMultipleSimulations()
    {
        var repeats = 20;
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        Enumerable.Range(0, repeats).ToList().ForEach(entry => RunSingleSimulation());
        stopwatch.Stop();
        Debug.LogWarning($"Mean time to simulate run is: {stopwatch.ElapsedMilliseconds/repeats} ms");
    }
    
    void RunSingleSimulation()
    {        
        var context = new SequenceContext(500, 3, 50, 8);
        var result = _simulator.Simulate(context);
        Debug.Log($"Score is              \t\t: {result.FinalScore}");
        Debug.Log($"TargetScore is       \t: {result.TargetScore}");
        Debug.Log($"BestPossibleResult is\t: {result.BestPossibleResult}");
        Debug.Log($"Readable score: {result.FinalScore.ParseToReadable()}");
        Debug.Log($"Time taken is: {result.CombinedSeconds}");
        Debug.Log($"-------------------------------------------");
    }
}
