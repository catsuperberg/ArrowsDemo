using DataAccess.DiskAccess.GameFolders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace AssetScripts.AssetCreation
{
    public class PermanentProjectileInjester : MonoBehaviour
    {        
        [SerializeField]
        BundleProjectilePrefabGenerator _prefabGenerator;
        IGameFolders _gameFolders = new EditorGameFolders();
        List<string> _projectileFolders; 
        List<string> _modelFiles; 
        List<SkinPackage> _skins; 
        
        RawModelLoader modelLoader = new RawModelLoader();
        
        public void InjestProjectiles()
        {            
            ScanForProjectileFolders();
            ScanForGLBModels();
            ConvertModelsToPrefabs();
        }
        
        void ScanForProjectileFolders()
        {
            var folderForProjectiles = Path.Combine(_gameFolders.AssetInjest, "Projectiles");
            _projectileFolders = Directory.GetDirectories(folderForProjectiles).ToList();
        }
        
        void ScanForGLBModels()
        {            
            _skins = new List<SkinPackage>();
            foreach(var folder in _projectileFolders)
                RegisterSkinIfValid(folder);
        }
        
        void RegisterSkinIfValid(string folderOfSpecificSkin)
        {
            var modelFile = Directory.GetFiles(folderOfSpecificSkin, "*.glb*").FirstOrDefault();
            var iconFile = Directory.GetFiles(folderOfSpecificSkin, "icon.png").FirstOrDefault();
            var metadataFile = Directory.GetFiles(folderOfSpecificSkin, "metadata.json").FirstOrDefault();
            if(metadataFile == null)
                metadataFile = CreateDefaultProjectileMetadata(folderOfSpecificSkin);
            _skins.Add(new SkinPackage(modelFile, iconFile, metadataFile));
        }
        
        string CreateDefaultProjectileMetadata(string folder)
        {
            Debug.Log("Creating default metadata file for projectile in folder: " + folder);
            var defaultMetadata = new ProjectileMetadata();
            var json = JsonUtility.ToJson(defaultMetadata, prettyPrint: true);
            var filePath = Path.Combine(folder, "metadata.json");
            FileStream stream = new FileStream(filePath, FileMode.Create);   
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();
            return filePath;   
        }
        
        void ConvertModelsToPrefabs()
        {
            var skinObjects = modelLoader.CreateInactiveGameObjects(_skins.Select(instance => instance.GLBModelPath));
            foreach(var skin in skinObjects)
                _prefabGenerator.CreateBundleProjectilesPrefab(skin);
        }
    }
}