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
using GameMath;


[TestFixture]
[RequiresPlayMode(false)]
public class PrefabIconGeneratorTests
{
    [SerializeField]
    GameObject Prefab;
       
    const string _testAssetsFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/TestAssets";
    const string _testResourcesFolder = "Assets/Code/Asset Scripts/Asset Creation/AssetCreationTests/Resources";
    const string _iconizerPrefabPath = "Assets/Code/Asset Scripts/Asset Creation/Iconizer.prefab";
    List<string> _pathsToModels;
    List<string> _skinNames;
    List<GameObject> _objectsToDestroy;
    RawModelLoader _loader;
    SkinPrefabGenerator _prefabGenerator;
    PrefabIconGenerator _iconGenerator;
    
    [SetUp]
    public void TestSetup()
    {
        var iconizerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_iconizerPrefabPath); 
        
        _iconGenerator = new PrefabIconGenerator();        
        _iconGenerator.Initialize(iconizerPrefab);        
        _loader = new RawModelLoader();
        _prefabGenerator = new SkinPrefabGenerator();    
        
        _pathsToModels = Directory.GetFiles(_testAssetsFolder, "*.glb").ToList();
        if(!_pathsToModels.Any())
            throw new FileLoadException("No models found");
        _skinNames = _pathsToModels.Select(Path.GetFileNameWithoutExtension).ToList();
                        
        _objectsToDestroy = new List<GameObject>();
            
        ClearResourceFolder();
    }
    
    [TearDown]
    public void TestTeardown()
    {        
        _objectsToDestroy.ForEach(GameObject.DestroyImmediate);
        ClearResourceFolder();
    }
    
    void ClearResourceFolder()
    {
        Directory.GetFiles(_testResourcesFolder).ToList().ForEach(File.Delete);
        Directory.GetDirectories(_testResourcesFolder).ToList().ForEach(entry => AssetDatabase.DeleteAsset(entry));
    }
    
    [Test]
    public void IconCreationTest()
    {
        var modelToIconize = CreateModelToIconize();
            
        var prefabPath = _prefabGenerator.CreatePrefab(modelToIconize, _testResourcesFolder);
        var iconPath = Path.Combine(_testResourcesFolder, "icon_" + modelToIconize.name + ".asset");     
               
        var prefab =  AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath); 
        var icon = _iconGenerator.CreatePrefabPreview(prefab);
        AssetDatabase.CreateAsset(icon, iconPath);
        
        Assert.That(prefabPath, Does.Exist);
        Assert.That(iconPath, Does.Exist);
        var iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);   
        Assert.That(iconTexture, Is.Not.Null);
    }
    
    GameObject CreateModelToIconize()
    {
        var pathToModel = _pathsToModels[GlobalRandom.RandomInt(0, _pathsToModels.Count)];
        var skinsInScene = _loader.CreateInactiveGameObjects(pathToModel.InList());   
        _objectsToDestroy.AddRange(skinsInScene); 
        return skinsInScene.First();
    }
}
