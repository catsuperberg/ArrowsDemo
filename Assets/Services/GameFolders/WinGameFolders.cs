using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GameStorage
{
    public class WinGameFolders :IGameFolders
    {        
        public string StreamingAssetsPath()
        {
            return Application.streamingAssetsPath;
        }
        
        public string SettingsFolder()
        {
            return getFolderOrCreateWhereAllowed("Settings Data");
        }
        
        public string SaveFolder()
        {
            return getFolderOrCreateWhereAllowed("Save Data");
        }
        
        private string getFolderOrCreateWhereAllowed(string folderName)
        {            
            string folder;
            folder = Path.Combine(Application.dataPath, folderName);
            if(!Directory.Exists(folder)){
                Directory.CreateDirectory(folder);
                if(!Directory.Exists(folder)){
                    folder = Application.persistentDataPath;
                }
            }
            return folder;
        }
    }
}
