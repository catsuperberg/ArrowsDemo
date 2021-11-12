using Level;
using Level.Track;
using GamePlay;
using GameMeta;
using Sequence;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{    
    [SerializeField]
    private ProceduralLevelManager _levelManager;
    // ISplineTrackProvider _splineTrackProvider;
    // ILevelManager _levelManager;
    
    public override void InstallBindings()
    {                
        Container.Bind<OperationExecutor>().AsSingle().NonLazy();
        Container.Bind<IMetaGame>().To<RandomSequenceGenerator>().AsSingle();
        Container.Bind<ISplineTrackProvider>().To<RandomizedSmoothTrackGenerator>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<ITrackPopulator>().To<TrackFiller>().FromNewComponentOnNewGameObject().AsSingle(); 
        Container.Bind<ITargerProvider>().To<TargetGroupGenerator>().FromNewComponentOnNewGameObject().AsSingle(); 
        Container.Bind<ITrackFollower>().To<SplineFollower>().FromNewComponentOnNewGameObject().AsSingle(); 
        Container.Bind<ILevelManager>().FromInstance(_levelManager).AsSingle();  
        
        Container.Bind<GameManager>().AsSingle().NonLazy();
    }
}