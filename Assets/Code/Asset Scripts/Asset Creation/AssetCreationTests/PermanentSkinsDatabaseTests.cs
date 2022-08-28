using AssetScripts.AssetCreation;
using Game.Gameplay.Meta.Skins;
using GameMath;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;


using System.Text;

[TestFixture]
[RequiresPlayMode(false)]
public class PermanentSkinsDatabaseTests
{
    const string _testAssetsFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/TestAssets/Crossbow";
    const string _testDatabaseJson = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/Resources/TestDatabase.json";
    const string _testResourcesFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/Resources";
    PermanentSkinsDatabase<ProjectileSkinData> _database;
    
    List<ProjectileSkinData> _testData1;
    List<ProjectileSkinData> _testData2;
        
    [SetUp]
    public void TestSetup()
    {
        ClearPrefabFolder();
        
        var allData = GenerateRandomData(3*2);
        var allDataInChunks = allData
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / (allData.Count/2))
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();
        _testData1 = allDataInChunks[0];
        _testData2 = allDataInChunks[1];
        
        _database = new PermanentSkinsDatabase<ProjectileSkinData>(_testDatabaseJson);            
    }
    
    List<ProjectileSkinData> GenerateRandomData(int numberOfEntries)
    {
        var tempData = new List<ProjectileSkinData>();
        while(tempData.Count < numberOfEntries)
            AddIfUnique(tempData, GetRandomData());
        return tempData;
    }
    
    void AddIfUnique(List<ProjectileSkinData> list, ProjectileSkinData data)
    {
        if(list.Any(entry => entry.Name == data.Name))
            return;
        list.Add(data);
    }
    
    ProjectileSkinData GetRandomData()
        => new ProjectileSkinData(
                name: GlobalRandom.RandomString(10),
                prefabPath: GlobalRandom.RandomString(10),
                iconPath: GlobalRandom.RandomString(10),
                baseCost: GlobalRandom.RandomIntInclusive(int.MaxValue),
                adWatchRequired: GlobalRandom.RandomBool());
    
    [TearDown]
    public void TestTeardown()
    {        
        // ClearPrefabFolder();
    }
    
    void ClearPrefabFolder()
    {
        Directory.GetFiles(_testResourcesFolder).ToList().ForEach(File.Delete);
        Directory.GetDirectories(_testResourcesFolder).ToList().ForEach(entry => AssetDatabase.DeleteAsset(entry));
    }
    
    [Test]
    public void AddSkinsUniqueByNameTest()
    {
        _database.AddSkinsUniqueByName(_testData1);
        Assert.That(_database.Skins, Is.EquivalentTo(_testData1));
        var oneOldOneNew = new List<ProjectileSkinData>{_testData1.First(), _testData2.First()};
        var expectedData = new List<ProjectileSkinData>(_testData1);
        expectedData.Add(_testData2.First());
        _database.AddSkinsUniqueByName(oneOldOneNew);        
        Assert.That(_database.Skins, Is.EquivalentTo(expectedData));
    }        
    
    [Test]
    public void SetSkinsData()
    {
        _database.SetSkinsDataKeepOldPropertiesOnNull(_testData1);
        Assert.That(_database.Skins, Is.EquivalentTo(_testData1));
        var modifiedEntry = new ProjectileSkinData(
                name: _testData1.First().Name,
                prefabPath: _testData2.First().PrefabPath,
                iconPath: _testData2.First().IconPath,
                baseCost: _testData2.First().BaseCost,
                adWatchRequired: _testData2.First().AdWatchRequired);
        _database.SetSkinsDataKeepOldPropertiesOnNull(new List<ProjectileSkinData>{modifiedEntry});
        var expectedData = new List<ProjectileSkinData>{modifiedEntry, _testData1[1], _testData1[2]};
        Assert.That(_database.Skins, Is.EquivalentTo(expectedData));
    }
    
    [Test]
    public void SaveToPermanentTest()
    {
        Assert.That(_testDatabaseJson, Does.Not.Exist);  
        _database.AddSkinsUniqueByName(_testData1);
        _database.SaveToPermanent();      
        Assert.That(_testDatabaseJson, Does.Exist);  
    }
    
    [Test]
    public void LoadDataFromFileTest()
    {  
        _database.AddSkinsUniqueByName(_testData1);
        _database.SaveToPermanent();      
        var newDatabase = new PermanentSkinsDatabase<ProjectileSkinData>(_testDatabaseJson);
        var newData = JsonConvert.SerializeObject(newDatabase.Skins);
        var oldData = JsonConvert.SerializeObject(_database.Skins);
        Assert.That(newData, Is.Not.EqualTo("{}"));
        Assert.That(newData, Is.Not.EqualTo("[]"));
        Assert.That(newData, Is.EquivalentTo(oldData));
    }
}
