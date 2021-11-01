using UnityEngine;
using Zenject;
using GamePlay;
using GameMeta;
using Sequence;

public class GameInstaller : MonoInstaller
{
    // [SerializeField]
    // SettingsMenu _settingMenuScript;
    
    public override void InstallBindings()
    {                
        Container.Bind<OperationExecutor>().AsSingle().NonLazy();
        Container.Bind<IMetaGame>().To<RandomSequenceGenerator>().AsSingle();
        Container.Bind<GameManager>().AsSingle().NonLazy();
        
        // Container.Bind<IDiskSerialization>().To<JsonDataStorage>().AsSingle();
        // Container.Bind<IGameFolders>().To<WinGameFolders>().AsSingle();
        // Container.Bind<IStreamingAssetsReader>().To<WinStreamingAssets>().AsSingle();
        // Container.Bind<ISettingsService>().To<SettingsService>().AsSingle();
        // // Container.Bind<SettingsService>().AsSingle().NonLazy();
        // Container.Bind<SettingsMenu>().FromInstance(_settingMenuScript);
    }
}