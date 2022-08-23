using AssetScripts.AssetCreation;
using DataAccess.DiskAccess.Serialization;
using Game.Gameplay.Meta.Skins;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
[RequiresPlayMode(false)]
public class PermanentSkinInjesterTests
{
    const string _testAssetsFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/TestAssets/FullInjestSet/Crossbow";
    const string _tempAssetsFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/TestAssets/Temp/Crossbow";
    const string _testUpdateAssetsFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/TestAssets/InjestSetWithUpdates/Crossbow";
    const string _testResourcesFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/Resources";
    const string _iconizerPrefabPath = "Assets/Code/Asset Scripts/Asset Creation/Iconizer.prefab";
    const string _testDatabaseJson = _testResourcesFolder + "/Crossbow.json";
    List<string> _skinNames;
    List<GameObject> _objectsToDestroy;
    PermanentSkinsDatabase<CrossbowSkinData> _database;
    CrossbowAssetMaker _assetMaker;
        
    SkinInjester<CrossbowSkinData, CrossbowInjestData> _injester;
    SkinInjester<CrossbowSkinData, CrossbowInjestData> _updateInjester;
    SkinInjester<CrossbowSkinData, CrossbowInjestData> _tempInjester;
    
    [SetUp]
    public void TestSetup()
    {
        ClearPrefabFolder();
        
        var loader = new RawModelLoader();
        var prefabGenerator = new SkinPrefabGenerator();
        var iconizerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_iconizerPrefabPath);         
        var iconGenerator = new PrefabIconGenerator();        
        iconGenerator.Initialize(iconizerPrefab);                
        _database = new PermanentSkinsDatabase<CrossbowSkinData>(_testDatabaseJson);
        
        var tempSkinFolder = CopyInjestAssetsToTempFolder(_skinWithOnlyData);
        
        var assetMaker = new CrossbowAssetMaker(loader, prefabGenerator, iconGenerator, (_testAssetsFolder, _testResourcesFolder));
        var updateAssetMaker = new CrossbowAssetMaker(loader, prefabGenerator, iconGenerator, (_testUpdateAssetsFolder, _testResourcesFolder));
        var tempAssetMaker = new CrossbowAssetMaker(loader, prefabGenerator, iconGenerator, (_tempAssetsFolder, _testResourcesFolder));
        var dataEnricher = new InjestDataEnricher<CrossbowSkinData, CrossbowInjestData>(_testAssetsFolder);
        var updateDataEnricher = new InjestDataEnricher<CrossbowSkinData, CrossbowInjestData>(_testUpdateAssetsFolder);
        var tempDataEnricher = new InjestDataEnricher<CrossbowSkinData, CrossbowInjestData>(_tempAssetsFolder);
        _injester = new SkinInjester<CrossbowSkinData, CrossbowInjestData>(assetMaker, _database, dataEnricher);
        _updateInjester = new SkinInjester<CrossbowSkinData, CrossbowInjestData>(updateAssetMaker, _database, updateDataEnricher);
        _tempInjester = new SkinInjester<CrossbowSkinData, CrossbowInjestData>(tempAssetMaker, _database, tempDataEnricher);
        
        _objectsToDestroy =  new List<GameObject>();
        _skinNames = Directory.GetDirectories(_testAssetsFolder).Select(entry => new DirectoryInfo(Path.GetFileName(entry)).Name).ToList();
    }
    
    [TearDown]
    public void TestTeardown()
    {        
        foreach(var entry in _objectsToDestroy)
            GameObject.DestroyImmediate(entry);
        ClearPrefabFolder();
    }
    
    void ClearPrefabFolder()
    {
        Directory.GetFiles(_testResourcesFolder).ToList().ForEach(File.Delete);
        Directory.GetDirectories(_testResourcesFolder).ToList().ForEach(entry => AssetDatabase.DeleteAsset(entry));
        Directory.GetDirectories(_tempAssetsFolder).ToList().ForEach(entry => AssetDatabase.DeleteAsset(entry));
    }
    
    [Test]
    public void InjestingInEmptyResourcesTest()
    {
        _injester.InjestSkins();
        
        VerifySkinResourceFoldersExist();
        VerifySkinFoldersAndFilesArentEmpty();
        VerifyIconsCreated();
        VerifyEntiesInDatabase();
    }
    
    void VerifySkinResourceFoldersExist()
    {        
        Assert.That(RealResourceFolders, Is.EquivalentTo(ExpectedResourceFolders));
    }
    
    void VerifySkinFoldersAndFilesArentEmpty()
    {
        Assert.That(RealResourceFolders.Select(Directory.GetFiles), Has.None.Empty);
        Assert.That(FilesInSkinFolders.Select(entry => new FileInfo(entry).Length), Has.None.Zero);
    }
        
    void VerifyIconsCreated()
    {
        var expectedIconFiles = ExpectedResourceFolders.Select(entry => Path.Combine(entry, IconNameForFolder(entry)));
        var realIconFiles = RealResourceFolders.SelectMany(entry => Directory.GetFiles(entry, "icon_*.asset"))
            .Select(entry => Path.ChangeExtension(entry, null));
        Assert.That(realIconFiles, Is.EquivalentTo(expectedIconFiles));
        Assert.That(realIconFiles, Has.None.Empty);
    }       
    
    void VerifyEntiesInDatabase()
    {
        Assert.That(_database._pathToDatabase, Does.Exist);
        var newDatabase = new PermanentSkinsDatabase<CrossbowSkinData>(_database._pathToDatabase);
        var skinNamesInDB = newDatabase.Skins.Select(entry => entry.Name);
        Assert.That(skinNamesInDB, Is.EquivalentTo(_skinNames));
    }
    
    [Test]
    public void UpdatingResourcesTest()
    {
        _injester.InjestSkins();
        var creationTimes = GetSkinPrefabUpdateTimes();
        _updateInjester.InjestSkins();      
        var updateTimes = GetSkinPrefabUpdateTimes();
        Assert.That(updateTimes[_skinWithFullUpdate], Is.GreaterThan(creationTimes[_skinWithFullUpdate]));
        Assert.That(updateTimes[_skinWithOnlyModel], Is.GreaterThan(creationTimes[_skinWithOnlyModel]));
        
        var updatedSkinData = JsonFile.GetObjectFromFile<List<CrossbowSkinData>>(_testDatabaseJson);
        VerifyDatabaseHasUpdatedEntry(_skinWithFullUpdate, updatedSkinData);
        VerifyDatabaseHasUpdatedEntry(_skinWithOnlyData, updatedSkinData);
    }    
    
    Dictionary<string, System.DateTime> GetSkinPrefabUpdateTimes()
        => _skinNames.ToDictionary(entry => entry, entry => UpdateTimeOfSkinPrefab(entry));
    
    System.DateTime UpdateTimeOfSkinPrefab(string skinName)
        => File.GetLastWriteTime(Directory.GetFiles(PathToSkinResources(skinName), "*.prefab").First());
    
    
    void VerifyDatabaseHasUpdatedEntry(string skinName, List<CrossbowSkinData> updatedData)
    {
        var injestData = JsonFile.GetObjectFromFile<CrossbowInjestData>(SkinInjestDataPath(skinName));
        Assert.That(updatedData.First(entry => entry.Name == skinName).BaseCost, Is.EqualTo(injestData.BaseCost));
        Assert.That(updatedData.First(entry => entry.Name == skinName).AdWatchRequired, Is.EqualTo(injestData.AdWatchRequired));
    }
    
    string SkinInjestDataPath(string skinName)
        => Directory.GetFiles(PathToSkinUpdateAssets(skinName), "injestData.*").First();
        
    // assert that exception happened
    [Test]
    public void OnlyDataTest()
    {
        Assert.That(_tempInjester.InjestSkins, Throws.Exception.TypeOf<ArgumentNullException>());
    }    
    
    // assert that default values provided for database entry
    [Test]
    public void OnlyModelTest()
    {
        var tempSkinFolder = CopyInjestAssetsToTempFolder(_skinWithOnlyModel);
        _tempInjester.InjestSkins();
        var defaultSkinData = new CrossbowSkinData();
        Assert.That(_database.Skins.First().BaseCost, Is.EqualTo(defaultSkinData.BaseCost));
        Assert.That(_database.Skins.First().AdWatchRequired, Is.EqualTo(defaultSkinData.AdWatchRequired));
    } 
    
    string CopyInjestAssetsToTempFolder(string skinName)
    {
        var tempSkinPath = Path.Combine(_tempAssetsFolder, skinName);
        FileUtil.CopyFileOrDirectory(PathToSkinUpdateAssets(skinName), tempSkinPath);
        return tempSkinPath;
    }
    
    string _skinWithFullUpdate = new DirectoryInfo(Directory.GetDirectories(_testUpdateAssetsFolder)
        .First(path => Directory.GetFiles(path, "*.glb").Any() && Directory.GetFiles(path, "injestData.*").Any())).Name;
    string _skinWithOnlyModel = new DirectoryInfo(Directory.GetDirectories(_testUpdateAssetsFolder)
        .First(path => Directory.GetFiles(path, "*.glb").Any() && !Directory.GetFiles(path, "injestData.*").Any())).Name;
    string _skinWithOnlyData = new DirectoryInfo(Directory.GetDirectories(_testUpdateAssetsFolder)
        .First(path => !Directory.GetFiles(path, "*.glb").Any() && Directory.GetFiles(path, "injestData.*").Any())).Name;
    private IEnumerable<string> FilesInSkinFolders => RealResourceFolders.SelectMany(Directory.GetFiles);
    IEnumerable<string> RealResourceFolders => Directory.GetDirectories(_testResourcesFolder);
    IEnumerable<string> ExpectedResourceFolders => _skinNames.Select(entry => Path.Combine(_testResourcesFolder, entry));
    string IconNameForFolder(string path) => "icon_" + new DirectoryInfo(path).Name;   
    string PathToSkinResources(string name) => Path.Combine(_testResourcesFolder, name);
    string PathToSkinInjestAssets(string name) => Path.Combine(_testAssetsFolder, name);
    string PathToSkinUpdateAssets(string name) => Path.Combine(_testUpdateAssetsFolder, name);
}
