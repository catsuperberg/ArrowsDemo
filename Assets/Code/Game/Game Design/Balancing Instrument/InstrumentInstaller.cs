using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using Game.GameDesign;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using System.Globalization;
using System.Numerics;
using Zenject;


public static class InstrumentInstaller
{
    public static void Compose(DiContainer container)
    {
        ComposeSequence(container);
        ComposeTargetGenerator(container);
        ComposePlayerActorFactories(container);
        container.Bind<RunSimulator>().AsTransient();
        var endConditions = new PlaythroughEndConditions(
            new System.TimeSpan(hours: 0, minutes: 40, seconds: 0), 
            new System.TimeSpan(hours: 0, minutes: 3, seconds: 0),
            BigInteger.Parse("1.0e20", NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint));
        container.Bind<PlaythroughEndConditions>().FromInstance(endConditions).AsTransient();
        ComposeFactoriesForPlaythroughFactory(container);
        container.Bind<PlaythroughSimulatorFactory>().AsTransient();
        container.Bind<DataRetriever>().AsTransient();
        container.Bind<DataPlotter>().AsTransient();
        container.Bind<BalanceController>().AsTransient();
    }
        
    static void ComposeSequence(DiContainer container)
    {        
        var folders = new GameFolders();
        var balanceConfig = JsonFile.LoadFromResources<GameBalanceConfiguration>(folders.ResourcesGameBalance, GameBalanceConfiguration.MainConfigurationName);
        container.Bind<GameBalanceConfiguration>().FromInstance(balanceConfig).AsSingle();
        container.Bind<IGameFolders>().FromInstance(folders).AsSingle();
        
        container.Bind<OperationProbabilitiesFactory>().AsTransient();
        container.Bind<OperationValueParametersFactory>().AsTransient();
        container.BindFactory<OperationFactory, OperationFactory.Factory>().NonLazy();         
        
        container.Bind<IOperationRules>().To<OperationRules>().AsTransient();
        container.Bind<ISequenceCalculator>().To<RandomSequenceGenerator>().AsTransient();
        container.Bind<ISequenceManager>().To<SequenceManager>().AsTransient();   
    }
    
    static void ComposeTargetGenerator(DiContainer container)
    {
        container.Bind<ITargetProvider>().To<TargetDataOnlyGenerator>().AsTransient();
    }
    
    static void ComposePlayerActorFactories(DiContainer container)
    {
        container.Bind<PriceCalculatorFactory>().AsTransient();
        container.Bind<SimpleUpgradePricing>().AsTransient();     
    }  
    
    static void ComposeFactoriesForPlaythroughFactory(DiContainer container)
    {
        container.Bind<UpgradeBuyerFactory>().AsTransient();  
        container.BindFactory<RunSimulator, RunSimulator.Factory>().NonLazy();         
    }
}