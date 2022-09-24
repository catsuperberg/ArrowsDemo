using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using DataManagement;
using Game.Gameplay.Meta;
using Game.Gameplay.Meta.Skins;
using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Crossbow;
using Game.Gameplay.Realtime.PlayfieldComponents.Target;
using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using Game.GameState;
using GameDesign;
using Game.Microinteracions;
using Settings;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{    
    [SerializeField]
    private ArrowsRunthroughFactory _runthroughFactory;
    [SerializeField]
    private ProjectileGenerator _projectileGenerator; 
    [SerializeField]
    private CrossbowGenerator _crossbowGenerator; 
    [SerializeField]
    private TargetGroupGenerator _targetGenerator; 
    [SerializeField]
    private AppStateFactory _appStateFactory;
    [SerializeField]
    AudioSource _musicSource;
    [SerializeField]
    AudioSource _SFXSource;
    [SerializeField]
    ScatterModels _backgroundScatterModels;
    [SerializeField]
    GameObject _debugOverlay;
    
    public override void InstallBindings()
    {      
        ComposeDiskAccess();
        ComposeUserContextRepository();  
        ComposeSettingsRepository();    
         
        ComposeSettings();   
        ComposeSkinsImport();
        ComposeUserContextManagement();
        ComposeMicrointeractions(); 
        ComposeAudio(); 
        ComposeSkinShop();    
        
        ComposeSequence();
        
        Container.Bind<ISplineTrackProvider>().To<RandomizedSmoothTrackGenerator>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<ITrackPopulator>().To<TrackFiller>().FromNewComponentOnNewGameObject().AsSingle(); 
        Container.Bind<ITargetProvider>().FromInstance(_targetGenerator).AsSingle();  
        Container.Bind<IProjectileProvider>().FromInstance(_projectileGenerator).AsSingle();  
        Container.Bind<ScatterModels>().FromInstance(_backgroundScatterModels).AsSingle();  
                                                 
        Container.Bind<ICrossbowProvider>().FromInstance(_crossbowGenerator).AsSingle(); 
        Container.Bind<ProjectileInPlaceReplacer>().AsSingle().NonLazy();  
        Container.Bind<CrossbowInPlaceReplacer>().AsSingle().NonLazy();                           
        Container.Bind<RunthroughContextManager>().AsSingle().NonLazy();
        Container.Bind<IRunthroughFactory>().FromInstance(_runthroughFactory).AsSingle();     
        Container.Bind<PreRunFactory>().AsSingle().NonLazy();        
        Container.Bind<RunthroughFactory>().AsSingle().NonLazy();    
        Container.Bind<IAppStateFactory>().FromInstance(_appStateFactory).AsSingle();  
    }
    
    void ComposeDiskAccess()
    {        
        Container.Bind<IDiskSerialization>().To<JsonDataStorage>().AsSingle();
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            Container.Bind<IStreamingAssetsReader>().To<WinStreamingAssets>().AsSingle();
        else if (Application.platform == RuntimePlatform.Android)
            Container.Bind<IStreamingAssetsReader>().To<AndroidStreamingAssets>().AsSingle();
        Container.Bind<IGameFolders>().To<GameFolders>().AsSingle();
    }
    
    void ComposeSkinsImport()
    {
        Container.Bind<ProjectileCollectionFactory>().AsSingle(); 
        Container.Bind<SkinCollection>().WithId("projectiles").FromResolveGetter<ProjectileCollectionFactory>(factory => factory.GetCurrentCollection()); 
        Container.Bind<CrossbowCollectionFactory>().AsSingle(); 
        Container.Bind<SkinCollection>().WithId("crossbows").FromResolveGetter<CrossbowCollectionFactory>(factory => factory.GetCurrentCollection()); 
    }
    
    void ComposeSkinShop()
    {
        var prjectileShopService = new SkinShopService(
            Container.TryResolveId<IRegistryAccessor>("userRegistryAccessor"), 
            Container.TryResolveId<SkinCollection>("projectiles"));
        Container.Bind<SkinShopService>().WithId("projectilesShop").FromInstance(prjectileShopService); 
        var crossbowShopService = new SkinShopService(
            Container.TryResolveId<IRegistryAccessor>("userRegistryAccessor"), 
            Container.TryResolveId<SkinCollection>("crossbows"));
        Container.Bind<SkinShopService>().WithId("crossbowsShop").FromInstance(crossbowShopService); 
    } 
    
    void ComposeUserContextRepository()
    {
        var gameFolders = new GameFolders();
        var RegistryFactory = new RegistryFactory(gameFolders);
        var nonVolatileStorage = new JsonStorage(new DiskAcessor());
        var _userRegistry = RegistryFactory.CreateRegistry("UserContext", nonVolatileStorage);
        Container.Bind<UserContextFactory>().AsSingle().NonLazy();
         
        Container.Bind<IRegistryManager>().WithId("userRegistryManager").FromInstance(_userRegistry.Manager).AsTransient(); 
        Container.Bind<IRegistryIngester>().WithId("userRegistryIngester").FromInstance(_userRegistry.Ingester).AsTransient(); 
        Container.Bind<IRegistryAccessor>().WithId("userRegistryAccessor").FromInstance(_userRegistry.Accessor).AsTransient(); 
        Container.Bind<IRegistryValueReader>().WithId("userRegistryAccessor").FromInstance(_userRegistry.Accessor).AsTransient();
    }
    
    void ComposeUserContextManagement()
    {
        Container.Bind<UserContext>().FromResolveGetter<UserContextFactory>(factory => factory.GetContext());
        Container.Bind<IUpgradeContextNotifier>().WithId("userContextNotifier").To<UserContextManager>().AsTransient();
        Container.Bind<ISkinContextNotifier>().WithId("userContextNotifier").To<UserContextManager>().AsTransient();
        Container.Bind<IContextProvider>().To<UserContextConverter>().AsSingle();                 
    }
    
    void ComposeSettingsRepository()
    {
        var gameFolders = new GameFolders();
        var RegistryFactory = new RegistryFactory(gameFolders);
        var nonVolatileStorage = new JsonStorage(new DiskAcessor());
        var _settingsRegistry = RegistryFactory.CreateRegistry("GameSettings", nonVolatileStorage);
        
        var settingsRegistryManager = new SettingsRegistryManager(_settingsRegistry.Manager, _settingsRegistry.Reader);
        Container.Bind<IUpdatedNotification>().WithId("settingsNotifier").FromInstance(settingsRegistryManager).AsTransient(); 
        Container.Bind<IRegistryIngester>().WithId("settingsIngester").FromInstance(_settingsRegistry.Ingester).AsTransient(); 
        Container.Bind<IRegistryAccessor>().WithId("settingsAccessor").FromInstance(_settingsRegistry.Accessor).AsTransient(); 
        Container.Bind<IRegistryValueReader>().WithId("settingsAccessor").FromInstance(_settingsRegistry.Accessor).AsTransient(); 
    }    
     
    void ComposeSettings()
    {
        Container.Bind<ResolutionScaling>().AsSingle().NonLazy();  
        Container.Bind<GraphicsPresset>().AsSingle().NonLazy(); 
        Container.Bind<MusicService>().AsSingle().NonLazy(); 
        Container.Bind<SFXService>().AsSingle().NonLazy(); 
        Container.Bind<GameObject>().WithId("DebugOverlay").FromInstance(_debugOverlay).AsTransient(); 
        Container.Bind<DebugOverlay>().AsSingle().NonLazy(); 
        
        Container.BindFactory<ProgressReset, ProgressReset.Factory>().FromNewComponentOnNewGameObject();
    }  
    
    void ComposeMicrointeractions()
    {
        Container.Bind<VibrationService>().AsSingle().NonLazy();
    }
    
    void ComposeAudio()
    {
        Container.Bind<AudioSource>().WithId("Music").FromInstance(_musicSource).AsTransient();  
        Container.Bind<AudioSource>().WithId("SFX").FromInstance(_SFXSource).AsTransient();  
    }    
    
    void ComposeSequence()
    {        
        Container.Bind<OperationProbabilitiesFactory>().AsSingle();
        Container.Bind<MathOperationProbabilities>()
            .FromResolveGetter<OperationProbabilitiesFactory>(factory => factory.GetFromGeneratedJson());
        Container.BindFactory<OperationFactory, OperationFactory.Factory>().NonLazy();         
        
        Container.Bind<OperationExecutor>().AsTransient().NonLazy();
        Container.Bind<ISequenceCalculator>().To<RandomSequenceGenerator>().AsSingle();
        Container.Bind<ISequenceManager>().To<SequenceManager>().AsSingle();    
    }
}