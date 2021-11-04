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
        
        //HACK pparently instantiating during install is bad, maybe just add managers to scene and get them through inspector 
        var splineTrackProvider = Container.InstantiateComponent<RandomizedSmoothTrackGenerator>(gameObject);
        Container.Bind<ISplineTrackProvider>().FromInstance(splineTrackProvider).AsSingle();  
        // var levelManager = Container.InstantiateComponent<ProceduralLevelManager>(gameObject);
        Container.Bind<ILevelManager>().FromInstance(_levelManager).AsSingle();  
        
        Container.Bind<GameManager>().AsSingle().NonLazy();
    }
}