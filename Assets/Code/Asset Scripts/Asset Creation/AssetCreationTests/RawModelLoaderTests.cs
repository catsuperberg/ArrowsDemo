using AssetScripts.AssetCreation;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;


[TestFixture]
[RequiresPlayMode(true)]
public class RawModelLoaderTests
{
    const string _testAssetsFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/TestAssets";
    List<string> _pathsToModels;
    List<GameObject> _objectsToDestroy;
    RawModelLoader _loader;
    
    [SetUp]
    public void TestSetup()
    {
        _objectsToDestroy =  new List<GameObject>();
        _loader = new RawModelLoader();
        _pathsToModels = Directory.GetFiles(_testAssetsFolder, "*.glb").ToList();
        if(!_pathsToModels.Any())
            throw new FileLoadException("No models found");
    }
    
    [TearDown]
    public void TestTeardown()
    {        
        _objectsToDestroy.ForEach(GameObject.DestroyImmediate);
    }
    
    [Test]
    [RequiresPlayMode(false)]
    public void CreateInactiveTest()
    {
        var createdObjects = _loader.CreateInactiveGameObjects(_pathsToModels);
        _objectsToDestroy.AddRange(createdObjects);
        Assert.That(createdObjects.Count, Is.EqualTo(_pathsToModels.Count));
    }
    
    [Test]
    [RequiresPlayMode(true)]
    public async Task CreateInactiveAsyncTest()
    {
        var creationTask = _loader.CreateInactiveGameObjectsAsync(_pathsToModels);
        var createdObjects = await creationTask ?? throw new System.Exception("no objects returned");
        _objectsToDestroy.AddRange(createdObjects);
        Assert.That(createdObjects.Count, Is.EqualTo(_pathsToModels.Count));
        Assert.That(createdObjects, Has.None.Null);
    }
    
    [Test]
    [RequiresPlayMode(true)]
    public async Task CreatedGameObjectsAreInactive()
    {
        var syncCreatedObjects = _loader.CreateInactiveGameObjects(_pathsToModels);
        Assert.That(syncCreatedObjects.Select(entry => entry.gameObject.activeSelf), Has.None.True);
        syncCreatedObjects.ForEach(GameObject.DestroyImmediate);
        var asyncCreatedObjects = await _loader.CreateInactiveGameObjectsAsync(_pathsToModels);
        Assert.That(asyncCreatedObjects.Select(entry => entry.gameObject.activeSelf), Has.None.True);
        asyncCreatedObjects.ForEach(GameObject.DestroyImmediate);
    }
    
    [Test]
    [RequiresPlayMode(true)]
    public async Task NamedGameObjets()
    {
        var fileDirections = _pathsToModels.Select(entry => (name: Path.GetFileNameWithoutExtension(entry) + "ArbitraryName", path: entry));
        var syncCreatedObjects = await _loader.CreateInactiveGameObjectsAsync(fileDirections);
        _objectsToDestroy.AddRange(syncCreatedObjects);
        Assert.That(syncCreatedObjects.Select(entry => entry.name), Is.EquivalentTo(fileDirections.Select(entry => entry.name)));
    }
}
