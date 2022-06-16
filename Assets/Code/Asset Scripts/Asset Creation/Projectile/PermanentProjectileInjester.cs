using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public class PermanentProjectileInjester : MonoBehaviour
    {        
        [SerializeField]
        BundleProjectilePrefabGenerator _prefabGenerator;
        IGameFolders _gameFolders = new EditorGameFolders();
        string _resourcesFolder = "Assets/Prefabs/Gameplay Items/Projectiles/Resources";
        List<string> _projectileFolders; 
        List<string> _modelFiles; 
        List<SkinPackage> _skinPackages;
        Dictionary<SkinPackage, GameObject> _skins; 
        List<ProjectileSkinData> _skinsData = new List<ProjectileSkinData>();
        
        RawModelLoader _modelLoader = new RawModelLoader();
        
        public void InjestProjectiles()
        {            
            var progressTimer = new System.Timers.Timer(800);
            progressTimer.Elapsed += (o, e) => {Debug.Log("Projectile injest in progress");};
            progressTimer.Start();
            
            _skinsData = new List<ProjectileSkinData>();
            ScanForProjectileFolders();
            ScanForGLBModels();
            InjestModels();
            UpdateSkinDatabase();
            
            AssetDatabase.Refresh();
            progressTimer.Dispose();
            Debug.Log("Projectile injest finished");
        }
        
        void ScanForProjectileFolders()
        {
            var folderForProjectiles = Path.Combine(_gameFolders.AssetInjest, "Projectiles");
            _projectileFolders = Directory.GetDirectories(folderForProjectiles).ToList();
        }
        
        void ScanForGLBModels()
        {            
            _skinPackages = new List<SkinPackage>();
            foreach(var folder in _projectileFolders)
                RegisterSkinIfValid(folder);
        }
        
        void RegisterSkinIfValid(string folderOfSpecificSkin)
        {
            var modelFile = Directory.GetFiles(folderOfSpecificSkin, "*.glb*").FirstOrDefault();
            var iconFile = Directory.GetFiles(folderOfSpecificSkin, "icon.png").FirstOrDefault();
            var injestDataFile = Directory.GetFiles(folderOfSpecificSkin, "*injestData.json*").FirstOrDefault();
            if(injestDataFile == null)
                injestDataFile = CreateDefaultProjectileInjestData(folderOfSpecificSkin);
            
            var name = Path.GetFileNameWithoutExtension(modelFile);
            _skinPackages.Add(new SkinPackage(name, modelFile, iconFile, injestDataFile));
        }
        
        string CreateDefaultProjectileInjestData(string folder)
        {
            Debug.Log("Creating default injestData file for projectile in folder: " + folder);            
            var defaultInjestData = new ProjectileInjestData();
            var json = JsonUtility.ToJson(defaultInjestData, prettyPrint: true);
            var filePath = Path.Combine(folder, "injestData.json");
            FileStream stream = new FileStream(filePath, FileMode.Create);   
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();
            return filePath;   
        }      
        
        void InjestModels()
        {
            var skinGOs = _modelLoader.CreateInactiveGameObjects(_skinPackages.Select(instance => instance.GLBModelPath));
            _skins = _skinPackages.Zip(skinGOs, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
                
            foreach(var skin in _skins)            
                InjestSkin(skin);
        }
        
        void InjestSkin(KeyValuePair<SkinPackage, GameObject> skin)
        {                              
            var skinFolder = GetSkinFolder(skin.Value.name);
            var skinPrefabPath = _prefabGenerator.CreateBundleProjectilesPrefab(skin.Value, skinFolder);
            _skinsData.Add(CreateSkinDataObject(skin.Key, skinPrefabPath));  
            CopyIcon(skin.Key, skinFolder);
        }
        
        ProjectileSkinData CreateSkinDataObject(SkinPackage skin, string skinPrefabPath)
        {
            var injestData = JsonFileOperations.GetDataObjectFromJsonFile<ProjectileInjestData>(skin.MetadataPath);
            var skinData = new ProjectileSkinData(
                name: skin.Name,
                resourcePath: skinPrefabPath.Replace(_resourcesFolder, ""),
                modelCheckSum: CalculateFileChecksum(skin.GLBModelPath),
                iconCheckSum: CalculateFileChecksum(skin.IconPath),
                baseCost: injestData.BaseCost,
                adWatchRequired: injestData.AdWatchRequired);
            return skinData;
        }
        
        void UpdateSkinDatabase()
        {
            ProjectileDatabase skinDatabase; 
            var skinDatabseName = "ProjectileDatabase.json";
            var skinDatabasePath = Path.Combine(_resourcesFolder, skinDatabseName);
            if(File.Exists(skinDatabasePath))
                skinDatabase = JsonFileOperations.GetDataObjectFromJsonFile<ProjectileDatabase>(skinDatabasePath);
            else
                skinDatabase = new ProjectileDatabase();
            
            skinDatabase.AddSkinsUniqueByName(_skinsData);   
            
            JsonFileOperations.SaveAsJson(skinDatabase, skinDatabasePath);
        }
        
        string CalculateFileChecksum(string path)
        {
            var algorithm = "SHA256";
            using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(algorithm))
			{
				using (var stream = System.IO.File.OpenRead(path))
				{
					var hash = hasher.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "");
				}
			}
        }
        
        string GetSkinFolder(string name)
        {
            var skinFolder = System.IO.Path.Combine(_resourcesFolder, name);
            if(!AssetDatabase.IsValidFolder(skinFolder))
                AssetDatabase.CreateFolder(_resourcesFolder, name);
            return skinFolder;
        }
        
        void CopyIcon(SkinPackage skin, string skinFolder)
        {
            var newFilePath = Path.Combine(skinFolder, "icon.png");
            System.IO.File.Copy(skin.IconPath, newFilePath, true);
        }
    }
}