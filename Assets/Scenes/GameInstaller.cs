using Level;
using Level.Track;
using Level.Track.Items;
using GameMeta;
using GamePlay;
using GameSettings;
using GameStorage;
using State;
using Sequence;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{    
    [SerializeField]
    private ProceduralLevelManager _levelManager;
    [SerializeField]
    private ProjectileGenerator _projectileGenerator; 
    [SerializeField]
    private TargetGroupGenerator _targetGenerator; 
    [SerializeField]
    private UI_StateManager _UI_Manager; 
    [SerializeField]
    private UIStateInputs _UIStateInputs; 
    [SerializeField]
    SettingsMenu _settingMenuScript;
    
    public override void InstallBindings()
    {                
        Container.Bind<OperationExecutor>().AsTransient().NonLazy();
        Container.Bind<IMetaGame>().To<RandomSequenceGenerator>().AsSingle();
        Container.Bind<IMetaManager>().To<MetaManager>().AsSingle();
        Container.Bind<ISplineTrackProvider>().To<RandomizedSmoothTrackGenerator>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<ITrackPopulator>().To<TrackFiller>().FromNewComponentOnNewGameObject().AsSingle(); 
        Container.Bind<ITargerProvider>().FromInstance(_targetGenerator).AsSingle();  
        Container.Bind<ITrackFollower>().To<SplineFollower>().FromNewComponentOnNewGameObject().AsSingle();  
        Container.Bind<IProjectileProvider>().FromInstance(_projectileGenerator).AsSingle();  
        Container.Bind<ILevelManager>().FromInstance(_levelManager).AsSingle();  
        
        Container.Bind<IGamePlayManager>().To<GamePlayManager>().AsSingle();
        
        Container.Bind(typeof(IStateChangeNotifier), typeof(IStateSignal)).To<GameState>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<ControlStateInputs>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<UIStateInputs>().FromInstance(_UIStateInputs).AsSingle();  
        
        Container.Bind<UI_StateManager>().FromInstance(_UI_Manager).AsSingle();  
        
        Container.Bind<IDiskSerialization>().To<JsonDataStorage>().AsSingle();
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            Container.Bind<IStreamingAssetsReader>().To<WinStreamingAssets>().AsSingle();
        else if (Application.platform == RuntimePlatform.Android)
            Container.Bind<IStreamingAssetsReader>().To<AndroidStreamingAssets>().AsSingle();
        Container.Bind<IGameFolders>().To<GameFolders>().AsSingle();
        Container.Bind<ISettingsExecutorService>().To<ChangedSettingsExecutorService>().AsSingle();
        
        Container.Bind<ISettingsService>().To<SettingsService>().AsSingle();
        Container.Bind<SettingsMenu>().FromInstance(_settingMenuScript);
    }
}