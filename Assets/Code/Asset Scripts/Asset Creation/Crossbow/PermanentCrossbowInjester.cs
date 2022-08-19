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
    public class PermanentCrossbowInjester
    {   
        string _folderWithSkins;
        string _resourcesFolder;
        RawModelLoader _modelLoader;
        SkinPrefabGenerator _prefabGenerator;
        PrefabIconGenerator _iconGenerator;
        PermanentSkinsDatabase<CrossbowSkinData> _database;
        
        IEnumerable<(string name, string pathToModel)> _modelsToSave;
        IEnumerable<(string name, string pathToData)> _skinDataToSet;
        IEnumerable<CrossbowSkinData> _skinAssetPaths;
        
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
        
        IEnumerable<(string name, string pathToModel)> SkinModelsToInjest()
        {
            var skinFolders = Directory.GetDirectories(_folderWithSkins).Where(entry => Directory.GetFiles(entry, "*.glb").Any());
            return skinFolders
                .Select(entry => (name: new DirectoryInfo(entry).Name, pathToModel: Directory.GetFiles(entry, "*.glb").FirstOrDefault()));
        }
        
        IEnumerable<(string name, string pathToData)> SkinDataToInjest()
        {
            var skinDataFolders = Directory.GetDirectories(_folderWithSkins).Where(entry => Directory.GetFiles(entry, "injestData.*").Any());
            return skinDataFolders
                .Select(entry => (name: new DirectoryInfo(entry).Name, pathToData: Directory.GetFiles(entry, "injestData.*").FirstOrDefault()));
        }
        
        void SavePrefabsWithAssets()
        {
            var modelsInScene = _modelLoader.CreateInactiveGameObjects(_modelsToSave);
            _modelsToSave.ToList().ForEach(entry => Directory.CreateDirectory(Path.Combine(_resourcesFolder, entry.name)));
            _modelsToSave.ToList().ForEach(entry => _prefabGenerator.CreatePrefab(
                modelsInScene.FirstOrDefault(GO => GO.name == entry.name), Path.Combine(_resourcesFolder, entry.name)));
            modelsInScene.ForEach(GameObject.DestroyImmediate);
        }
        
        void GenerateIconFiles()
        {
            var iconsToSave = _modelsToSave.Select(entry => 
                (name: entry.name, texture: _iconGenerator.CreatePrefabPreview(LoadPrefabByname(entry.name))));
            iconsToSave.ToList().ForEach(entry => SaveAsSpriteAsset(entry.texture, Path.Combine(_resourcesFolder, Path.Combine(entry.name, "icon_" + entry.name + ".asset"))));            
        }
        
        void SaveAsSpriteAsset(Texture2D texture, string pathToSaveAt)
        {
            var sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.zero,100);
            AssetDatabase.CreateAsset(texture, pathToSaveAt);
            AssetDatabase.AddObjectToAsset(sprite, pathToSaveAt);
        }
        
        GameObject LoadPrefabByname(string name) => Resources.Load<GameObject>(Path.Combine(name, name));
        
        void AssingCreatedAssetsToSkins()
        {
            var skinFolders = _modelsToSave.Select(entry => Path.Combine(_resourcesFolder, entry.name)).ToList();
            _skinAssetPaths = skinFolders.Select(entry => AssignAssetPath(entry));
        }
        
       CrossbowSkinData  AssignAssetPath(string assetFolder)
        {
            var name = new DirectoryInfo(Path.GetFileName(assetFolder)).Name;
            var prefabPath = Directory.GetFiles(assetFolder, "*.prefab").First();
            var iconPath = Directory.GetFiles(assetFolder, "icon_*").First();
            return new CrossbowSkinData(name,prefabPath,iconPath, null, null);
        }
        
        void SetDatabaseEntries()
        {
            var dataEnricher = new InjestDataEnricher<CrossbowSkinData, CrossbowInjestData>();
            
            var dataForSkins = dataEnricher.GetInjestDataForAllSkins(_skinAssetPaths, _skinDataToSet);
            _database.SetSkinsDataKeepOldPropertiesOnNull(dataForSkins);
            _database.SaveToPermanent();
        }
    }
}

#endif