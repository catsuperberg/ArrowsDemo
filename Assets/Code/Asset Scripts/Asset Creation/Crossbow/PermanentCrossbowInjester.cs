using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Meta.Skins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR

namespace AssetScripts.AssetCreation
{
    public class PermanentCrossbowInjester : MonoBehaviour
    {   
        string _folderWithSkins;
        string _resourcesFolder;
        RawModelLoader _modelLoader;
        SkinPrefabGenerator _prefabGenerator;
        PrefabIconGenerator _iconGenerator;
        PermanentSkinsDatabase<CrossbowSkinData> _database;
        
        IEnumerable<(string skinName, string pathToModel)> _modelsToSave;
        IEnumerable<(string skinName, string pathToData)> _skinDataToSet;
        IEnumerable<(string skinName, string pathToPrefab, string pathToIcon)> _skinAssetPaths;
        
        public PermanentCrossbowInjester(RawModelLoader modelLoader, SkinPrefabGenerator prefabGenerator,
            PrefabIconGenerator iconGenerator, PermanentSkinsDatabase<CrossbowSkinData> database,
            string folderWithSkins = "Assets/Asset Injest/Crossbow", string finalResourceFolder = "Assets/Prefabs/Gameplay Items/Crossbow/Resources")
        {
            _folderWithSkins = folderWithSkins;
            _resourcesFolder = finalResourceFolder; 
            _modelLoader = modelLoader ?? throw new ArgumentNullException(nameof(modelLoader));
            _prefabGenerator = prefabGenerator ?? throw new ArgumentNullException(nameof(prefabGenerator));
            _iconGenerator = iconGenerator ?? throw new ArgumentNullException(nameof(iconGenerator));
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        
        public void InjestCrossbows()
        {
            _modelsToSave = SkinModelsToInjest();
            _skinDataToSet = SkinDataToInjest();
            SavePrefabsWithAssets();
            GenerateIconFiles();
            AssingCreatedAssetsToSkins();
            SetDatabaseEntries();
            AssetDatabase.Refresh();
        }
        
        IEnumerable<(string skinName, string pathToModel)> SkinModelsToInjest()
        {
            var skinFolders = Directory.GetDirectories(_folderWithSkins).Where(entry => Directory.GetFiles(entry, "*.glb").Any());
            return skinFolders
                .Select(entry => (skinName: new DirectoryInfo(entry).Name, pathToModel: Directory.GetFiles(entry, "*.glb").FirstOrDefault()));
        }
        
        IEnumerable<(string skinName, string pathToData)> SkinDataToInjest()
        {
            var skinDataFolders = Directory.GetDirectories(_folderWithSkins).Where(entry => Directory.GetFiles(entry, "injestData.*").Any());
            return skinDataFolders
                .Select(entry => (skinName: new DirectoryInfo(entry).Name, pathToData: Directory.GetFiles(entry, "injestData.*").FirstOrDefault()));
        }
        
        void SavePrefabsWithAssets()
        {
            var modelsInScene = _modelLoader.CreateInactiveGameObjects(_modelsToSave);
            _modelsToSave.ToList().ForEach(entry => Directory.CreateDirectory(Path.Combine(_resourcesFolder, entry.skinName)));
            _modelsToSave.ToList().ForEach(entry => _prefabGenerator.CreatePrefab(
                modelsInScene.FirstOrDefault(GO => GO.name == entry.skinName), Path.Combine(_resourcesFolder, entry.skinName)));
            modelsInScene.ForEach(GameObject.DestroyImmediate);
        }
        
        void GenerateIconFiles()
        {
            var iconsToSave = _modelsToSave.Select(entry => 
                (skinName: entry.skinName, texture: _iconGenerator.CreatePrefabPreview(LoadPrefabBySkinName(entry.skinName))));
            iconsToSave.ToList().ForEach(entry => SaveAsSpriteAsset(entry.texture, Path.Combine(_resourcesFolder, Path.Combine(entry.skinName, "icon_" + entry.skinName + ".asset"))));            
        }
        
        void SaveAsSpriteAsset(Texture2D texture, string pathToSaveAt)
        {
            var sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.zero,100);
            AssetDatabase.CreateAsset(texture, pathToSaveAt);
            AssetDatabase.AddObjectToAsset(sprite, pathToSaveAt);
        }
        
        GameObject LoadPrefabBySkinName(string name) => Resources.Load<GameObject>(Path.Combine(name, name));
        
        void AssingCreatedAssetsToSkins()
        {
            var skinFolders = _modelsToSave.Select(entry => Path.Combine(_resourcesFolder, entry.skinName)).ToList();
            _skinAssetPaths = skinFolders.Select(entry => AssignAssetPath(entry));
        }
        
        (string skinName, string pathToPrefab, string pathToIcon) AssignAssetPath(string assetFolder)
        {
            var skinName = new DirectoryInfo(Path.GetFileName(assetFolder)).Name;
            var prefabPath = Directory.GetFiles(assetFolder, "*.prefab").First();
            var iconPath = Directory.GetFiles(assetFolder, "icon_*").First();
            return (skinName,prefabPath,iconPath);
        }
        
        void SetDatabaseEntries()
        {
            var dataForSkins = GetInjestDataForAllSkins();
            _database.SetSkinsDataKeepOldPropertiesOnNull(dataForSkins);
            _database.SaveToPermanent();
        }
        
        List<CrossbowSkinData> GetInjestDataForAllSkins()
        {
            var data = new List<CrossbowSkinData>();
            var skinsWithModelAndData = _modelsToSave.Select(entry => entry.skinName)
                .Where(name => _skinDataToSet.Any(data => data.skinName == name));
            var skinsWithOnlyData = _skinDataToSet.Select(entry => entry.skinName).Except(skinsWithModelAndData);
            var skinsWithOnlyModel = _modelsToSave.Select(entry => entry.skinName).Except(skinsWithModelAndData);
            data.AddRange(GenerateDataFullSkin(skinsWithModelAndData));    
            if(skinsWithOnlyData.Any())
                data.AddRange(GenerateDataDataOnly(skinsWithOnlyData));
            if(skinsWithOnlyModel.Any())                        
                data.AddRange(GenerateDataModelOnly(skinsWithOnlyModel));    
            return data;
        }
        
        List<CrossbowSkinData> GenerateDataModelOnly(IEnumerable<string> skinNames)
            => skinNames.Select(entry => new CrossbowSkinData(
                    name: entry,
                    prefabPath: _skinAssetPaths.First(asset => asset.skinName == entry).pathToPrefab,
                    iconPath: _skinAssetPaths.First(asset => asset.skinName == entry).pathToIcon,
                    baseCost: null,
                    adWatchRequired: null))
                    .ToList();
                
            
        List<CrossbowSkinData> GenerateDataDataOnly(IEnumerable<string> skinNames)
        {
            var injestData = skinNames
                .Select(entry => (skinNames: entry, 
                    injestData: JsonFileOperations.GetObjectFromJsonFile<CrossbowInjestData>(_skinDataToSet.First(skin => skin.skinName == entry).pathToData)));
            return skinNames.Select(entry => new CrossbowSkinData(
                        name: entry,
                        prefabPath: null,
                        iconPath: null,
                        baseCost: injestData.First(data => data.skinNames == entry).injestData.BaseCost,
                        adWatchRequired: injestData.First(data => data.skinNames == entry).injestData.AdWatchRequired))
                        .ToList();
        }
        
        List<CrossbowSkinData> GenerateDataFullSkin(IEnumerable<string> skinNames)
        {
            var injestData = skinNames
                .Select(entry => (skinNames: entry, 
                    injestData: JsonFileOperations.GetObjectFromJsonFile<CrossbowInjestData>(_skinDataToSet.First(skin => skin.skinName == entry).pathToData)));
            return skinNames.Select(entry => new CrossbowSkinData(
                        name: entry,
                        prefabPath: _skinAssetPaths.First(asset => asset.skinName == entry).pathToPrefab,
                        iconPath: _skinAssetPaths.First(asset => asset.skinName == entry).pathToIcon,
                        baseCost: injestData.First(data => data.skinNames == entry).injestData.BaseCost,
                        adWatchRequired: injestData.First(data => data.skinNames == entry).injestData.AdWatchRequired))
                        .ToList();
        }
    }
}

#endif