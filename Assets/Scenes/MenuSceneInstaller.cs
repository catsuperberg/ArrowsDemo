using UnityEngine;
using Zenject;
using GameStorage;
using GameSettings;

public class MenuSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IDiskSerialization>().To<JsonDataStorage>().AsSingle();
        Container.Bind<IGameFolders>().To<WinGameFolders>().AsSingle();
        Container.Bind<IStreamingAssetsReader>().To<WinStreamingAssets>().AsSingle();
        Container.Bind<SettingsService>().AsSingle().NonLazy();
    }
}