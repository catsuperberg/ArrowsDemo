using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GameStorage;

namespace GameSettings
{
    public enum AudioVisualOptions
    {
        MasterVolume,
        MusicVolume,
        SfxVolume,
        Vibration,
        ResolutionScaling,
        Graphics,
        High
    }
    
    public class SettingsService : ISettingsService
    {     
        private readonly IGameFolders _gameFolders;
        private readonly IDiskSerialization _dataStorage;
        
        private string defaultsName = "settings_default";
        private string currentName = "settings";
        
        SettingsService(IGameFolders folderStrutureImplimentation, IDiskSerialization storageAccessImplimentation)
        {
            if(folderStrutureImplimentation == null)
                throw new System.Exception("IGameFolders isn't provided to Settings");
            if(storageAccessImplimentation == null)
                throw new System.Exception("IDiskSerialization isn't provided to Settings");
                
            _gameFolders = folderStrutureImplimentation;
            _dataStorage = storageAccessImplimentation;
            
            MakeSureSettingsFolderIsPopulated();
        }
           
        public void SetOption<T>(AudioVisualOptions optionToSet, T value)
        {
            
        }
        
        public void ApplySettings()
        {
            
        }
        
        public void SaveSettings()
        {
            
        }
        
        private void MakeSureSettingsFolderIsPopulated()
        {            
            if(!_dataStorage.FileExists(_gameFolders.SettingsFolder(), defaultsName))
            {    
                Debug.LogWarning("No default settings file found at |" + Path.Combine(_gameFolders.SettingsFolder(), defaultsName) + "| , creating from default at streamingAssets");
                createDefaultSettingsFile();     
            }
            if(!_dataStorage.FileExists(_gameFolders.SettingsFolder(), currentName))
            {
                Debug.LogWarning("No settings file found at |" + Path.Combine(_gameFolders.SettingsFolder(), currentName) + "| , creating from default");
                createSettingsFileFromDefault();
            }
        }
        
        private void createDefaultSettingsFile()
        {
            var settingsFolderName = new DirectoryInfo(_gameFolders.SettingsFolder()).Name;
            _dataStorage.CreateFileFromStreamingAssets<SettingsData>(settingsFolderName, defaultsName, _gameFolders.SettingsFolder(), defaultsName);
        }
        
        private void createSettingsFileFromDefault()
        {
            _dataStorage.CopyFileData(_gameFolders.SettingsFolder(), defaultsName, _gameFolders.SettingsFolder(), currentName);
        }
    }    
}

