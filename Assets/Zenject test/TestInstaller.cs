using System.Collections;
using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var logger = new DebugLogger();
        Container.Bind<IMessageWriter>().FromInstance(logger);
        // Container.Bind<IMessageWriter>().To<WarningDebugLogger>().AsSingle();
        Container.Bind<Greeter>().AsSingle().NonLazy();
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

public class WarningDebugLogger : IMessageWriter
{
    public void WriteMassage(string message)
    {
        Debug.LogWarning(message);
    }
}