using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using Game.GameDesign;
using Game.Gameplay.Meta.Shop;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using Zenject;


public static class InstrumentInstaller
{
    public static void Compose(DiContainer container)
    {
        ComposeSequence(container);
        ComposeTargetGenerator(container);
        ComposePlayerActorFactories(container);
        container.Bind<RunSimulator>().AsTransient();
        ComposeFactoriesForPlaythroughFactory(container);
        container.Bind<PlaythroughSimulatorFactory>().AsTransient();
        container.Bind<DataRetriever>().AsTransient();
        container.Bind<DataPlotter>().AsTransient();
        container.Bind<DataProcessing>().AsTransient();
        container.Bind<BalanceController>().AsTransient();
    }
        
    static void ComposeSequence(DiContainer container)
    {        
        var folders = new GameFolders();
        var balanceConfig = JsonFile.LoadFromResources<GameBalanceConfiguration>(folders.ResourcesGameBalance, GameBalanceConfiguration.MainConfigurationName);
        container.Bind<GameBalanceConfiguration>().FromInstance(balanceConfig).AsSingle();
        container.Bind<IGameFolders>().FromInstance(folders).AsSingle();
        
        container.Bind<OperationProbabilitiesFactory>().AsSingle();
        container.Bind<OperationValueParametersFactory>().AsSingle();
        container.BindFactory<OperationFactory, OperationFactory.Factory>().AsSingle();         
        
        container.Bind<IOperationRules>().To<OperationRules>().AsSingle();
        container.Bind<ISequenceCalculator>().To<RandomSequenceGenerator>().AsTransient();
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