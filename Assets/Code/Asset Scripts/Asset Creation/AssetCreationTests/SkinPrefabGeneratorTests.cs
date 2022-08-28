using AssetScripts.AssetCreation;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Utils;


[TestFixture]
[RequiresPlayMode(true)]
public class SkinPrefabGeneratorTests
{
    const string _testAssetsFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/TestAssets";
    const string _testResourcesFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/Resources";
    List<string> _pathsToModels;
    List<string> _skinNames;
    List<GameObject> _objectsToDestroy;
    RawModelLoader _loader;
    SkinPrefabGenerator _prefabGenerator;
    
    [SetUp]
    public void TestSetup()
    {
        _objectsToDestroy =  new List<GameObject>();
        _loader = new RawModelLoader();
        _prefabGenerator = new SkinPrefabGenerator();
        _pathsToModels = Directory.GetFiles(_testAssetsFolder, "*.glb").ToList();
        _skinNames = _pathsToModels.Select(Path.GetFileNameWithoutExtension).ToList();
        if(!_pathsToModels.Any())
            throw new FileLoadException("No models found");
            
        ClearResourceFolder();
    }
    
    [TearDown]
    public void TestTeardown()
    {        
        foreach(var entry in _objectsToDestroy)
            GameObject.DestroyImmediate(entry);
        ClearResourceFolder();
    }
    
    void ClearResourceFolder()
    {
        Directory.GetFiles(_testResourcesFolder).ToList().ForEach(File.Delete);
        Directory.GetDirectories(_testResourcesFolder).ToList().ForEach(entry => AssetDatabase.DeleteAsset(entry));
    }
    
    [Test]
    public async Task UpdateExistingPrefabsTest()
    {
        var pathToModel = _pathsToModels.First();
        var originalWriteTimes = await CreatePrefabAndGetDateWriten(pathToModel);
        await Task.Delay(500);
        var updateWriteTimes = await CreatePrefabAndGetDateWriten(pathToModel);
        Assert.That(updateWriteTimes.Count(), Is.EqualTo(originalWriteTimes.Count()), "More files after update");
        Assert.That(updateWriteTimes.Select(updated => originalWriteTimes.Any(original => original >= updated)), Has.None.True, "No files updated for skin");        
    }
    
    async Task<IEnumerable<System.DateTime>> CreatePrefabAndGetDateWriten(string modelPath)
    {
        var skinsInScene = await _loader.CreateInactiveGameObjectsAsync(modelPath.InList());        
        CreatePrefabInItsFolder(skinsInScene.First());
        var writeTimes = FilesInSkinFolders.Where(entry => !entry.Contains("meta")).Select(File.GetLastWriteTime).ToList();
        skinsInScene.ForEach(GameObject.DestroyImmediate);            
        return writeTimes;
    }
    
    [Test]
    public async Task CreateNewPrefabsTest()
    {
        var sceneObjects = await CreateObjectsInScene();
        _objectsToDestroy.AddRange(sceneObjects);
        sceneObjects.ForEach(CreatePrefabInItsFolder);
            
        VerifySkinResourceFoldersExist();
        VerifySkinFoldersAndFilesArentEmpty();
        VerifyPrefabResourcesCreated(sceneObjects);
        VerifyPrefabNamesAsExpected();
    }
    
    async Task<List<GameObject>> CreateObjectsInScene()
    {
        var creationTask = _loader.CreateInactiveGameObjectsAsync(_pathsToModels);
        return await creationTask ?? throw new System.Exception("no objects returned");
    } 
    
    void CreatePrefabInItsFolder(GameObject objectToTurnIntoPrefab)
    {
        var skinFolderPath = Path.Combine(_testResourcesFolder, objectToTurnIntoPrefab.name);
        var skinFolder = Directory.Exists(skinFolderPath) ? skinFolderPath : AssetDatabase.CreateFolder(_testResourcesFolder, objectToTurnIntoPrefab.name);
        _prefabGenerator.CreatePrefab(objectToTurnIntoPrefab, skinFolderPath);
    }
    
    void VerifySkinResourceFoldersExist()
    {        
        var expectedResourceFolders = _skinNames.Select(entry => Path.Combine(_testResourcesFolder, entry));
        Assert.That(RealPrefabFolders, Is.EquivalentTo(expectedResourceFolders));
    }
    
    void VerifySkinFoldersAndFilesArentEmpty()
    {
        Assert.That(RealPrefabFolders.Select(Directory.GetFiles), Has.None.Empty);
        Assert.That(FilesInSkinFolders.Select(entry => new FileInfo(entry).Length), Has.None.Zero);
    }

    void VerifyPrefabResourcesCreated(List<GameObject> skinGOs)
    {
        var expectedMeshes = _skinNames.Select(entry => "mesh_" + entry);
        var expectedMaterials = _skinNames.Select(entry => "material_" + entry);
        var realFileNames = FilesInSkinFolders.Select(Path.GetFileNameWithoutExtension);
        var expectedTextures = GetExpectedTexturesFromSkins(skinGOs);
        Assert.That(expectedMeshes, Is.SubsetOf(realFileNames));
        Assert.That(expectedMaterials, Is.SubsetOf(realFileNames));
        Assert.That(expectedTextures, Is.SubsetOf(realFileNames));
    }
    
    IEnumerable<string> GetExpectedTexturesFromSkins(List<GameObject> skinGOs)
    {
        var namesOfSkinsWithTextures = skinGOs.Where(GO => GO.GetComponent<Renderer>()?.sharedMaterial.GetTexture("_MainTex") != null).Select(GO => GO.name);
        return namesOfSkinsWithTextures.Select(entry => "texture_" + entry);  
    }
    
    void VerifyPrefabNamesAsExpected()
    {
        var expectedPrefabs = _skinNames;
        var realFileNames = FilesInSkinFolders.Select(Path.GetFileNameWithoutExtension);
        Assert.That(expectedPrefabs, Is.SubsetOf(realFileNames));
    }
    
    private IEnumerable<string> FilesInSkinFolders
        => RealPrefabFolders.SelectMany(Directory.GetFiles);
        
    string[] RealPrefabFolders => Directory.GetDirectories(_testResourcesFolder);
}
