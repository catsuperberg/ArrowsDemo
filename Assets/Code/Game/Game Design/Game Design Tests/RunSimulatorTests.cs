using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using ExtensionMethods;
using Game.GameDesign;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using NUnit.Framework;
using System.Linq;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;
using Zenject;

using Debug = UnityEngine.Debug;


[TestFixture]
public class RunSimulatorTests : ZenjectUnitTestFixture
{
    RunSimulator _simulator;
    
    [SetUp]
    public void TestSetup()
    {
        ComposeSequence();
        ComposeTargetGenerator();
        ComposePlayer();
        Container.Bind<RunSimulator>().AsTransient();
        _simulator = Container.Resolve<RunSimulator>();
    }
        
    void ComposeSequence()
    {        
        var folders = new GameFolders();
        var balanceConfig = JsonFile.LoadFromResources<GameBalanceConfiguration>(folders.ResourcesGameBalance, GameBalanceConfiguration.MainConfigurationName);
        Container.Bind<GameBalanceConfiguration>().FromInstance(balanceConfig).AsSingle();
        Container.Bind<IGameFolders>().FromInstance(folders).AsSingle();
        
        Container.Bind<OperationProbabilitiesFactory>().AsTransient();
        Container.Bind<OperationValueParametersFactory>().AsTransient();
        Container.BindFactory<OperationFactory, OperationFactory.Factory>().NonLazy();         
        
        Container.Bind<IOperationRules>().To<OperationRules>().AsTransient();
        Container.Bind<ISequenceCalculator>().To<RandomSequenceGenerator>().AsTransient();
        Container.Bind<ISequenceManager>().To<SequenceManager>().AsTransient();   
    }
    
    void ComposeTargetGenerator()
    {
        Container.Bind<ITargetProvider>().To<TargetDataOnlyGenerator>().AsTransient();
    }
    
    void ComposePlayer()
    {
        var gateSelector = new GateSelector(GateSelectors.GoodPlayer.Chance());
        var adSelector = new AdSkipper();
        Container.Bind<PriceCalculatorFactory>().AsTransient();
        Container.Bind<SimpleUpgradePricing>().AsTransient();
        Container.Bind<HighestPriceBuyer>().AsTransient();
        var upgradeBuyer = Container.Resolve<HighestPriceBuyer>();
        
        var actors = new PlayerActors(gateSelector, adSelector, upgradeBuyer);
        var player = new VirtualPlayer(actors);
        Container.Bind<VirtualPlayer>().FromInstance(player).AsTransient(); 
    }    
    
    [Test, Performance, RequiresPlayMode(false)]
    public void SimulationPerformanceTest()
    {
        Measure.Method(() => RunSingleSimulation())
            .WarmupCount(5)
            .MeasurementCount(25)
            .Run();
    }
    
    
    [Test, Performance, RequiresPlayMode(false)]
    public void TestSingleThreadedPerformance()
    {        
        var simultationsToDo = 50;
        var range = Enumerable.Range(0, simultationsToDo);
        Measure.Method(() => range
                                .Select(entry => RunSingleSimulation())
                                .ToList())
            .WarmupCount(5)
            .MeasurementCount(25)
            .Run();
    }
    
    [Test, Performance, RequiresPlayMode(false)]
    public void TestMulithreadedPerformance()
    {
        var simultationsToDo = 50;
        var range = Enumerable.Range(0, simultationsToDo);
        var simulators = range.Select(entry => Container.Resolve<RunSimulator>()).ToList();
        var threads = UnityEngine.Mathf.Clamp((int)((System.Environment.ProcessorCount/2)-1), 3, int.MaxValue);
        Measure.Method(() => simulators
                                .AsParallel()
                                .WithDegreeOfParallelism(threads)
                                .Select(simulator => RunSingleSimulation(simulator))
                                .ToList())
            .WarmupCount(5)
            .MeasurementCount(25)
            .Run();
    }
    
    [Test, RequiresPlayMode(false)]
    public void RunMultipleSimulations()
    {
        var repeats = 20;
        var dataPoints = Enumerable.Range(0, repeats).Select(entry => RunSingleSimulation());
        dataPoints.ToList().ForEach(PrintSimulationData);
    }
    
    RunData RunSingleSimulation()
    {        
        var context = new SequenceContext(500, 3, 50, 8);
        var result = _simulator.Simulate(context, Container.Resolve<VirtualPlayer>().Actors);
        return result;
    }
    
    
    RunData RunSingleSimulation(RunSimulator simulator)
    {        
        var context = new SequenceContext(500, 3, 50, 8);
        var result = simulator.Simulate(context, Container.Resolve<VirtualPlayer>().Actors);
        return result;
    }
    
    void PrintSimulationData(RunData data)
    {        
        Debug.Log($"Score is              \t\t: {data.FinalScore}");
        Debug.Log($"TargetScore is       \t: {data.TargetScore}");
        Debug.Log($"BestPossibleResult is\t: {data.BestPossibleResult}");
        Debug.Log($"Readable score: {data.FinalScore.ParseToReadable()}");
        Debug.Log($"Time taken is: {data.CombinedTime}");
        Debug.Log($"-------------------------------------------");
    }
}
