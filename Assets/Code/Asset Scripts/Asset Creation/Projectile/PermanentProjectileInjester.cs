using DataAccess.DiskAccess.GameFolders;
using DataAccess.DiskAccess.Serialization;
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
        
        RawModelLoader _modelLoader = new RawModelLoader();
        
        public void InjestProjectiles()
        {            
            ScanForProjectileFolders();
            ScanForGLBModels();
            InjestModels();
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
            _skinPackages.Add(new SkinPackage(modelFile, iconFile, injestDataFile));
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
            CreateSkinDataObject(skin, skinFolder);  
            _prefabGenerator.CreateBundleProjectilesPrefab(skin.Value, skinFolder);
            CopyIcon(skin.Key, skinFolder);
        }
        
        void CreateSkinDataObject(KeyValuePair<SkinPackage, GameObject> skin, string skinFolder)
        {
            var injestData = JsonFileOperations.GetDataObjectFromJsonFile<ProjectileInjestData>(skin.Key.MetadataPath);
            var skinData = new ProjectileSkinData(
                name: skin.Value.name,
                modelCheckSum: CalculateFileChecksum(skin.Key.GLBModelPath),
                iconCheckSum: CalculateFileChecksum(skin.Key.IconPath),
                baseCost: injestData.BaseCost,
                adWatchRequired: injestData.AdWatchRequired);
            JsonFileOperations.SaveAsJson(skinData, Path.Combine(skinFolder, skin.Value.name + ".json"));
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