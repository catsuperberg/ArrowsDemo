using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace DataAccess.DiskAccess.GameFolders
{
    public class GameFolders :IGameFolders
    {        
        public string SettingsFolder {get; private set;}
        public string SaveFolder {get; private set;}
        public string StreamingAssetsPath {get; private set;}
        
        private string _baseFolder;
        
        public GameFolders()
        {
            _baseFolder = GetValidBaseFolder();
                                
            SettingsFolder = getFolderOrCreateWhereAllowed("Settings Data");
            SaveFolder = getFolderOrCreateWhereAllowed("Save Data");
            StreamingAssetsPath = Application.streamingAssetsPath;
            
            Debug.Log("========== GAME FOLDERS ==========");
            Debug.Log("SettingsFolder: " + SettingsFolder);
            Debug.Log("SaveFolder: " + SaveFolder);
            Debug.Log("StreamingAssetsPath: " + StreamingAssetsPath);
        }
        
        private string getFolderOrCreateWhereAllowed(string folderName)
        {          
            var fullFolder = Path.Combine(_baseFolder, folderName);
            Uri fullFolderUri;
            if(Uri.TryCreate(fullFolder, UriKind.RelativeOrAbsolute, out fullFolderUri))
                if(Directory.Exists(fullFolder))
                {
                    Debug.Log("Path already exists, returning it: " + fullFolder);
                    return fullFolder;
                }
                else
                {
                    var folderCreated = TryToCreateFolder(fullFolder);
                    if(folderCreated)
                    {
                        Debug.Log("Path sucessfully created, returning it: " + fullFolder);
                        return fullFolder;                        
                    }
                    else
                    {
                        Debug.Log("Couldn't create path: " + fullFolder);
                        Debug.Log("Substituting with Application.persistentDataPath");
                        return Application.persistentDataPath;
                    }
                }
            else
            {
                Debug.Log("Uri: " + fullFolder);
                Debug.Log("Uri isn't valid, substituting with Application.persistentDataPath");
                return Application.persistentDataPath;                
            }
        }
        
        bool TryToCreateFolder(string folder)
        {
            bool folderCreationFailed = false;
            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception e)
            {
                Debug.Log("Can't create folder, exception: " + e.ToString());
                folderCreationFailed = true;
            }
            
            return !folderCreationFailed;
        }
        
        string GetValidBaseFolder()
        {            
            
            Debug.Log("======= Deciding on base folder =======");
            var dataPathIsAFile = new Uri(Application.dataPath).AbsolutePath.Split('/').Last().Contains('.');
            Uri dataPathUri;
            if(dataPathIsAFile || !Uri.TryCreate(Application.dataPath, UriKind.RelativeOrAbsolute, out dataPathUri))
            {
                Debug.Log("Substituting Application.persistentDataPath as Application.dataPath isn't valid: " + Application.dataPath);
                Debug.Log("Application.persistentDataPath is: " + Application.dataPath);
                return Application.persistentDataPath;                
            }
            else
            {
                Debug.Log("Application.dataPath is valid: " + Application.dataPath);
                return Application.dataPath;
            }
        }
    }
}
