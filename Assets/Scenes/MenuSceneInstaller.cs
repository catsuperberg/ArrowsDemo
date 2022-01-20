using UnityEngine;
using Zenject;
using GameStorage;
using GameSettings;

public class MenuSceneInstaller : MonoInstaller
{
    [SerializeField]
    SettingsMenu _settingMenuScript;
    
    public override void InstallBindings()
    {        
        Container.Bind<IDiskSerialization>().To<JsonDataStorage>().AsSingle();
        Container.Bind<IGameFolders>().To<GameFolders>().AsSingle();
        Container.Bind<IStreamingAssetsReader>().To<WinStreamingAssets>().AsSingle();
        Container.Bind<ISettingsService>().To<SettingsService>().AsSingle();
        Container.Bind<SettingsMenu>().FromInstance(_settingMenuScript);
        // Container.Bind<SettingsService>().AsSingle().NonLazy();
    }
}