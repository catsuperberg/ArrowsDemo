using Level;
using Level.Track;
using Level.Track.Items;
using GameMeta;
using GamePlay;
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
    
    public override void InstallBindings()
    {                
        Container.Bind<OperationExecutor>().AsTransient().NonLazy();
        Container.Bind<IMetaGame>().To<RandomSequenceGenerator>().AsSingle();
        Container.Bind<ISplineTrackProvider>().To<RandomizedSmoothTrackGenerator>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<ITrackPopulator>().To<TrackFiller>().FromNewComponentOnNewGameObject().AsSingle(); 
        Container.Bind<ITargerProvider>().To<TargetGroupGenerator>().FromNewComponentOnNewGameObject().AsSingle(); 
        Container.Bind<ITrackFollower>().To<SplineFollower>().FromNewComponentOnNewGameObject().AsSingle();  
        Container.Bind<IProjectileProvider>().FromInstance(_projectileGenerator).AsSingle();  
        Container.Bind<ILevelManager>().FromInstance(_levelManager).AsSingle();  
        
        Container.Bind<IGamePlayManager>().To<GamePlayManager>().AsSingle();
        
        Container.Bind<GameState>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}