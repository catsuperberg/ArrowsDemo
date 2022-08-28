using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace DataAccess.DiskAccess.GameFolders
{
    public class EditorGameFolders :IGameFolders
    {        
        public string SettingsFolder {get; private set;}
        public string SaveFolder {get; private set;}
        public string StreamingAssetsPath {get; private set;}
        public string AssetInjest {get; private set;}
        
        private string _baseFolder;
        
        public EditorGameFolders()
        {
            SettingsFolder = "Assets/Settings Data";
            SaveFolder = "Assets/Save Data";
            AssetInjest = "Assets/Asset Injest";
            StreamingAssetsPath = "Assets/StreamingAssetsPath";
            
            Debug.Log("========== GAME FOLDERS ==========");
            Debug.Log("SettingsFolder: " + SettingsFolder);
            Debug.Log("SaveFolder: " + SaveFolder);
            Debug.Log("StreamingAssetsPath: " + StreamingAssetsPath);
            Debug.Log("AssetInjest: " + AssetInjest);
        }
    }
}
