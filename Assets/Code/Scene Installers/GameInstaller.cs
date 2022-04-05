using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using DataManagement;
using Game.Gameplay.Meta;
using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using Game.GameState;
using Settings;
using UI;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{    
    [SerializeField]
    private ArrowsRuntimeFactory _runtimeFactory;
    [SerializeField]
    private ProjectileGenerator _projectileGenerator; 
    [SerializeField]
    private TargetGroupGenerator _targetGenerator; 
    [SerializeField]
    private AppStateFactory _appStateFactory;
    
    public override void InstallBindings()
    {                
        Container.Bind<OperationExecutor>().AsTransient().NonLazy();
        Container.Bind<ISequenceCalculator>().To<RandomSequenceGenerator>().AsSingle();
        Container.Bind<ISequenceManager>().To<SequenceManager>().AsSingle();
        Container.Bind<ISplineTrackProvider>().To<RandomizedSmoothTrackGenerator>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<ITrackPopulator>().To<TrackFiller>().FromNewComponentOnNewGameObject().AsSingle(); 
        Container.Bind<ITargetProvider>().FromInstance(_targetGenerator).AsSingle();  
        Container.Bind<IProjectileProvider>().FromInstance(_projectileGenerator).AsSingle();  
                
        Container.Bind<IDiskSerialization>().To<JsonDataStorage>().AsSingle();
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            Container.Bind<IStreamingAssetsReader>().To<WinStreamingAssets>().AsSingle();
        else if (Application.platform == RuntimePlatform.Android)
            Container.Bind<IStreamingAssetsReader>().To<AndroidStreamingAssets>().AsSingle();
        Container.Bind<IGameFolders>().To<GameFolders>().AsSingle();
        Container.Bind<ISettingsExecutorService>().To<ChangedSettingsExecutorService>().AsSingle();
        
        Container.Bind<ISettingsService>().To<SettingsService>().AsSingle();
        
        
        ComposeUserContextManagement();        
                                 
        Container.Bind<IRuntimeFactory>().FromInstance(_runtimeFactory).AsSingle();     
        Container.Bind<PreRunFactory>().AsSingle().NonLazy();        
        Container.Bind<RunthroughFactory>().AsSingle().NonLazy();    
        Container.Bind<IAppStateFactory>().FromInstance(_appStateFactory).AsSingle();  
    }
    
    void ComposeUserContextManagement()
    {
        var gameFolders = new GameFolders();
        var RegistryFactory = new RegistryFactory(gameFolders);
        var nonVolatileStorage = new JsonStorage(new DiskAcessor());
        var _userRegistry = RegistryFactory.CreateRegistry("UserContext", nonVolatileStorage);
        
        var userContextManager = new UserContextManager(_userRegistry.Ingester, _userRegistry.Manager);
        var userContextConverter = new UserContextConverter(_userRegistry.Reader);
        Container.Bind<IContextProvider>().FromInstance(userContextConverter).AsSingle(); 
        Container.Bind<IUpdatedNotification>().WithId("userContextNotifier").FromInstance(userContextManager).AsSingle(); 
        Container.Bind<IRegistryAccessor>().WithId("userRegistryAccessor").FromInstance(_userRegistry.Accessor).AsSingle(); 
        Container.Bind<IRegistryValueReader>().WithId("userRegistryAccessor").FromInstance(_userRegistry.Accessor).AsSingle(); 
    }
}