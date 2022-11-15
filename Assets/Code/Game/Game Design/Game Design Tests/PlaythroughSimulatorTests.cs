using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using ExtensionMethods;
using Game.GameDesign;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using NUnit.Framework;
using System.Globalization;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

using Debug = UnityEngine.Debug;


[TestFixture]
public class PlaythroughSimulatorTests : ZenjectUnitTestFixture
{            
    UpgradeBuyerFactory _buyerFactory;
    
    [SetUp]
    public void TestSetup()
    {
        ComposeSequence();
        ComposeTargetGenerator();
        ComposePlayerActorFactories();
        Container.Bind<RunSimulator>().AsTransient();
        var endConditions = new PlaythroughEndConditions(
            new System.TimeSpan(hours: 0, minutes: 40, seconds: 0), 
            new System.TimeSpan(hours: 0, minutes: 3, seconds: 0),
            BigInteger.Parse("1.0e20", NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));
        Container.Bind<PlaythroughEndConditions>().FromInstance(endConditions).AsTransient();
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
        Container.Bind<ISequenceManager>().To<SequenceManager>().AsSingle();   
    }
    
    void ComposeTargetGenerator()
    {
        Container.Bind<ITargetProvider>().To<TargetDataOnlyGenerator>().AsTransient();
    }
    
    void ComposePlayerActorFactories()
    {
        Container.Bind<PriceCalculatorFactory>().AsTransient();
        Container.Bind<SimpleUpgradePricing>().AsTransient();
        Container.Bind<UpgradeBuyerFactory>().AsTransient();
        _buyerFactory = Container.Resolve<UpgradeBuyerFactory>();        
    }  
    
    
    PlaythroughSimulator CreatePlaythrough()
    {        
        var rand = new System.Random(this.GetHashCode() + System.DateTime.Now.GetHashCode());
        var gateSelector = new GateSelector(GateSelectorGrades.GetRandomGradeChance(rand));
        var adSelector = AdSelectorGrades.GetRandomGrade(rand);
        
        var upgradeBuyer = _buyerFactory.GetRandomGrade(rand);         
        var actors = new PlayerActors(gateSelector, adSelector, upgradeBuyer);
        var player = new VirtualPlayer(actors);
        return new PlaythroughSimulator(player, Container.Resolve<RunSimulator>(), Container.Resolve<PlaythroughEndConditions>());
    }
    
    
    [Test, RequiresPlayMode(false)]
    public void SimulateSinglePlaythrough()
    {        
        var playthrough = CreatePlaythrough();
        var result = playthrough.Simulate();
        PrintPlaythroughInfo(result); 
    }    
    
    [Test, RequiresPlayMode(false)]
    public void SimulatePlaythroughsMultithreaded()
    {        
        var numberOfPlaytrhoughs = 80;
        var threads = Mathf.Clamp((int)((System.Environment.ProcessorCount/2)-1), 3, int.MaxValue);
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        var playthroughs = Enumerable.Range(0, numberOfPlaytrhoughs)
            .Select(entry => CreatePlaythrough())
            .ToList();
        var results = playthroughs
            .AsParallel()
            .WithDegreeOfParallelism(threads)
            .Select(playthrough => playthrough.Simulate())
            .ToList();
        stopwatch.Stop();
        
        Debug.Log("");
        Debug.Log("MULTITHREADED PLAYTHORUGH SIMULATION RESULTS");
        var timePer = stopwatch.Elapsed/numberOfPlaytrhoughs;
        Debug.Log($"time per simulation: {timePer}");
        Debug.Log("");
        results.ForEach(PrintPlaythroughInfo);
    }   
    
    void PrintPlaythroughInfo(PlaythroughData info)
    {                
        Debug.Log(info.PlayerHeader);        
        Debug.Log("");
        Debug.Log($"Runs per playthrough: {info.NumberOfRuns}");
        Debug.Log($"Time to finish: {info.CombinedTime}");
        Debug.Log($"Final reward: {info.Runs.Last().FinalScore.ParseToReadable()}");   
        var reasonString = info.FinishReasons.Aggregate("", (text, entry) => text += $"{entry};");
        Debug.Log($"Reasons to finish: {reasonString}");   
        Debug.Log("");   
        
        var maxReward = info.Runs.Last().FinalScore;
        var rewardValues = PlaythroughData.LogarithmicRewardsList(maxReward, 8);
        var rewardTimes = info.TimeToRewards(rewardValues);
        var timesToRewardString = rewardTimes.Aggregate("", (text, kvp) =>
            text += $"{kvp.Key.ParseToReadable()} - {kvp.Value:mm\\:ss}; ");        
        Debug.Log($"Time to get reward: {timesToRewardString}");   
        var upgradesString = info.UpgradesPerRun.Aggregate("", (text, entry) => 
            text += (text != "") ? $",{entry}" : entry);
        Debug.Log($"Upgrades per run: {upgradesString}");    
        Debug.Log("");    
        
        
        var TimePerRunString = info.LevelTimePerRun.Aggregate("", (text, entry) => 
            text += (text != "") ? $", {entry.TotalSeconds.ToString("N1")} s" : $"{entry.TotalSeconds.ToString("N1")} s");
        Debug.Log($"Time to finish level: {TimePerRunString}");   
        var FailTimeString = info.TimePerRun.Aggregate("", (text, entry) => 
            text += (text != "") ? $", {entry.TotalSeconds.ToString("N1")} s" : $"{entry.TotalSeconds.ToString("N1")} s");
        Debug.Log($"Run time with fails: {FailTimeString}");   
        Debug.Log("============================================");   
    }    
}
