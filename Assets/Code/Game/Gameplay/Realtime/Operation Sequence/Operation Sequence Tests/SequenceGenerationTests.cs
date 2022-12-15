using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using ExtensionMethods;
using Game.GameDesign;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.PerformanceTesting;
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
        var balanceConfig = JsonFile.LoadFromResources<GameBalanceConfiguration>(folders.ResourcesGameBalance, GameBalanceConfiguration.MainConfigurationName);
        Container.Bind<GameBalanceConfiguration>().FromInstance(balanceConfig).AsSingle();
        
        Container.Bind<OperationProbabilitiesFactory>().AsTransient();
        Container.Bind<OperationValueParametersFactory>().AsTransient();
        Container.BindFactory<OperationFactory, OperationFactory.Factory>().NonLazy();         
        
        Container.Bind<IOperationRules>().To<OperationRules>().AsTransient();
        Container.Bind<ISequenceCalculator>().To<RandomSequenceGenerator>().AsSingle();
        Container.Bind<ISequenceManager>().To<SequenceManager>().AsSingle();    
    }
    
    [Test, RequiresPlayMode(false)]
    public void MakeGameBalanceConfiguration()
    {
        var folders = new GameFolders();
        var config = new GameBalanceConfiguration(1, 1, 200, 1, 1);
        JsonFile.SaveAsJson(config, folders.ResourcesGameBalance, GameBalanceConfiguration.MainConfigurationName);
    }
    
    [Test, Performance, RequiresPlayMode(false)]
    public void TestLateAverageGenerationPerformance()
    {        
        var context = new SequenceContext(500, 3, 50, 8);
        var generator = Container.Resolve<ISequenceCalculator>();
        
        Measure.Method(() => {generator.GetAverageSequenceResult(context);})
            .WarmupCount(10)
            .MeasurementCount(500)
            .Run();
    }
    
    [Test, Performance, RequiresPlayMode(false)]
    public void TestEarlyAverageGenerationPerformance()
    {        
        var context = new SequenceContext(500, 3, 8, 8);
        var generator = Container.Resolve<ISequenceCalculator>();
        
        Measure.Method(() => {generator.GetAverageSequenceResult(context);})
            .WarmupCount(10)
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
    
    [Test, RequiresPlayMode(false)]
    public void FastLessThanOneReplacementTest()
    {
        var repeats = 10;
        var sequences = CreateSequences(repeats);
        sequences.ToList().ForEach(CheckIfLessThanOnePossible);
    }
    
    IEnumerable<OperationPairsSequence> CreateSequences(int repeats)
    {        
        var initValueRange = Enumerable.Range(1, 100);
        var operationsValueRange = Enumerable.Range(5, 100);
        var contextes = new List<SequenceContext>();
        foreach(var initialValue in initValueRange)
            foreach(var operationCount in operationsValueRange)
                contextes.Add(new SequenceContext(500, initialValue, operationCount, 8));
        var generator = Container.Resolve<ISequenceCalculator>();       
         
        var contexesWithRepeats = Enumerable.Range(0, repeats).SelectMany(entry => contextes);
        
        return contexesWithRepeats
            .AsParallel()
            .Select(context => generator.GetRandomSequence(context));
    }
    
    void CheckIfLessThanOnePossible(OperationPairsSequence sequence)
    {
        sequence.Sequence
            .Aggregate(sequence.InitValue, (accumulator, pair) => 
                {
                    accumulator = pair.BestResult(accumulator); 
                    Assert.That(accumulator > 0, "Mini math replacement produces values less than 1");
                    return accumulator;
                });
    }
    
    void GenerateSequnce(BigInteger target, ISequenceCalculator generator, SequenceContext context)
    {
        var spread = 15;
        OperationPairsSequence sequence = default(OperationPairsSequence);
        
        try
        {
            sequence = generator.GetSequenceInSpreadRange(target, spread, context);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Coulnd't generate sequence");
            Debug.Log(ex);
            throw ex;
        }
                
        var absoluteSpread = BigInteger.Abs(target - sequence.BestPossibleResult);
        var percentSpread = BigInteger.Multiply(absoluteSpread, new BigInteger(100))/target;
        Assert.That(percentSpread <= spread, $"Actuall spread is higher then requested. Requested: {spread}, result: {percentSpread}");
        Debug.Log($"Sequence result: {sequence.BestPossibleResult.ParseToReadable()} Abs spread: {absoluteSpread.ParseToReadable()} Spread percents: {percentSpread}");
    }  
    
    [Test, RequiresPlayMode(false)]
    public void TestOperationRulesIDHash()
    {
        var rules = Container.Resolve<IOperationRules>();
        var initialValues = Enumerable.Range(1, (short)rules.MinInitless)
            .Select(value => (short)value);
        var leftRightPairs = Enumerable.Range(1, 0x9F)
            .Select(value => (short)value)
            .Select(leftValue => Enumerable.Range(1, 0x9F)
            .Select(value => (short)value)
                .Select(rightValue => (left: leftValue, right: rightValue)))
            .SelectMany(entry => entry);
            
        var hashes = initialValues
            .Select(initValue => leftRightPairs.Select(pair => OperationRules.IdHash(pair.left, pair.right, initValue)))
            .SelectMany(entry => entry)
            .ToList();
            
        var repeats = hashes.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
        
        Assert.That(repeats, Is.Empty);
    }
}
