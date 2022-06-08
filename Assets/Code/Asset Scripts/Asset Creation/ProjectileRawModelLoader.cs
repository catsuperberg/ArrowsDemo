using DataAccess.DiskAccess.GameFolders;
using Siccity.GLTFUtility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public class ProjectileRawModelLoader
    {        
        IGameFolders _gameFolders;
        
        public ProjectileRawModelLoader(IGameFolders gameFolders)
        {
            if(gameFolders == null)
                throw new ArgumentNullException("IGameFolders isn't provided to " + this.GetType().Name);
                
            _gameFolders = gameFolders;
            var settings = new ImportSettings();
            Importer.ImportGLBAsync(FilePath(gameFolders), settings, onFinished: OnFinishAsync);
        }   
        
        public GameObject LoadPencil()
        {
            var GO = Importer.LoadFromFile(FilePath(_gameFolders));
            GO.SetActive(false);
            return GO;
        }
        
        string FilePath(IGameFolders gameFolders)
        {
            return (gameFolders.StreamingAssetsPath + "/Projectiles/Pencil/Pencil.glb");
        }
             
        void OnFinishAsync(GameObject result, AnimationClip[] animations)
        {
            Debug.LogWarning("Finished importing " + result.name);
            result.SetActive(false);
        }
    }
}