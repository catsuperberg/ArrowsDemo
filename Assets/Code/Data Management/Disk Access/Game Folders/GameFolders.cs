using System;
using System.Linq;
using UnityEngine;
using System.IO;

namespace DataAccess.DiskAccess.GameFolders
{
    public class GameFolders : IGameFolders
    {        
        public string SettingsFolder {get; private set;}
        public string SaveFolder {get; private set;}
        public string StreamingAssetsPath {get; private set;}
        public string AssetInjest {get; private set;}
        
        public string ResourcesGameBalance {get => "Assets/Code/Game/Game Design/Resources/Game Balance";}
        
        private string _baseFolder;
        
        public GameFolders()
        {
            _baseFolder = GetValidBaseFolder();
                                
            SettingsFolder = getFolderOrCreateWhereAllowed("Settings Data");
            SaveFolder = getFolderOrCreateWhereAllowed("Save Data");
            AssetInjest = getFolderOrCreateWhereAllowed("Runtime Injest");
            StreamingAssetsPath = Application.streamingAssetsPath;
            
            Debug.Log("========== GAME FOLDERS ==========");
            Debug.Log("SettingsFolder: " + SettingsFolder);
            Debug.Log("SaveFolder: " + SaveFolder);
            Debug.Log("StreamingAssetsPath: " + StreamingAssetsPath);
            Debug.Log("AssetInjest: " + AssetInjest);
        }
        
        private string getFolderOrCreateWhereAllowed(string folderName)
        {          
            var fullFolder = Path.Combine(_baseFolder, folderName);
            Uri fullFolderUri;
            if(Uri.TryCreate(fullFolder, UriKind.RelativeOrAbsolute, out fullFolderUri))
                if(Directory.Exists(fullFolder))
                    return fullFolder;
                else
                {
                    var folderCreated = TryToCreateFolder(fullFolder);
                    if(folderCreated)
                        return fullFolder;     
                    else
                        return Application.persistentDataPath;
                }
            else
                return Application.persistentDataPath;        
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
            var dataPathIsAFile = new Uri(Application.dataPath).AbsolutePath.Split('/').Last().Contains('.');
            Uri dataPathUri;
            if(dataPathIsAFile || !Uri.TryCreate(Application.dataPath, UriKind.RelativeOrAbsolute, out dataPathUri))
                return Application.persistentDataPath;                
            else
                return Application.dataPath;
        }
    }
}
