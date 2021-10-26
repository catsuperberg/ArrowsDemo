using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GameStorage;

namespace GameSettings
{
    public class SettingsService : ISettingsService
    {     
        private readonly IGameFolders _gameFolders;
        private readonly IDiskSerialization _dataStorage;
        
        private SettingsData _currentSettings;
        private SettingsData _newSettings;
        
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
            LoadSettings();
        }
           
        public void SetOption<T>(AudioVisualOptions optionToSet, T value)
        {
            Debug.Log("Option to set: " + Enum.GetName(typeof(AudioVisualOptions), optionToSet)
                + "\tvalue: " + value);
            // switch (optionToSet)
            // {
            //     case AudioVisualOptions.MasterVolume:
            //         _newSettings.MasterVolume = Convert.ToSingle(value);
            //         return;
            //     case AudioVisualOptions.MusicVolume:
            //         _newSettings.MusicVolume = Convert.ToSingle(value);
            //         return;
            //     case AudioVisualOptions.SfxVolume:
            //         _newSettings.SfxVolume = Convert.ToSingle(value);
            //         return;
            //     case AudioVisualOptions.Vibration:
            //         _newSettings.Vibration = Convert.ToBoolean(value);
            //         return;
            //     case AudioVisualOptions.Graphics:
            //         _newSettings.Graphics = Convert.ToInt32(value);
            //         return;
            //     default:
            //         Debug.LogWarning("No such option as: " + Enum.GetName(typeof(AudioVisualOptions), optionToSet));
            //         return;
            // }
            ApplySettings();
        }
        
        // TODO: Write code to actually apply recieved settings
        public void ApplySettings()
        {
            
        }
        
        public void SaveSettings()
        {
            _dataStorage.SaveToDisk(_currentSettings, _gameFolders.SettingsFolder(), currentName);
        }
        
        public void LoadSettings()
        {
            _currentSettings = _dataStorage.GetDataObjectFromFile<SettingsData>(_gameFolders.SettingsFolder(), currentName);
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

