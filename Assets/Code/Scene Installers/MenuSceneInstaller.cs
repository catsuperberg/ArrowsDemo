using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using Settings;
using UI;
using UnityEngine;
using Zenject;

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
    }
}