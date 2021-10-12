using System.Collections;
using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Greeter>().AsSingle().NonLazy();
        Container.Bind<IMessageWriter>().To<DebugLogger>().AsSingle();
    }
}

public class Greeter
{
    IMessageWriter _massageService;
    
    public Greeter(IMessageWriter service)
    {
        _massageService = service;
        service.WriteMassage("Hello with DI!");
    }
}

public interface IMessageWriter
{
    void WriteMassage(string message);
}

public class DebugLogger : IMessageWriter
{
    public void WriteMassage(string message)
    {
        Debug.Log(message);
    }
}