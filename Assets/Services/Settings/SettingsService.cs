using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using GameStorage;

namespace GameSettings
{
    public class SettingsService : ISettingsService
    {     
        private readonly IGameFolders _gameFolders;
        private readonly IDiskSerialization _dataStorage;
        private readonly ISettingsExecutorService _settingsExecutor;
        
        private SettingsContainer _settings;
        private HashSet<Settings> _settingsToChange = new HashSet<Settings>();
        
        private string defaultsName = "settings_default";
        private string currentName = "settings";
        
        SettingsService(IGameFolders folderStrutureImplimentation, IDiskSerialization storageAccessImplimentation, ISettingsExecutorService settingsExecutor)
        {
            if(folderStrutureImplimentation == null)
                throw new System.Exception("IGameFolders isn't provided to Settings");
            if(storageAccessImplimentation == null)
                throw new System.Exception("IDiskSerialization isn't provided to Settings");
            if(settingsExecutor == null)
                throw new System.Exception("ISettingsExecutorService isn't provided to Settings");
                
            _gameFolders = folderStrutureImplimentation;
            _dataStorage = storageAccessImplimentation;
            _settingsExecutor = settingsExecutor;
            
            MakeSureSettingsFolderIsPopulated();
            LoadSettings();
            ApplySettings();
        }
           
        public void SetOption<T>(Settings settingToSet, T value) where T : IComparable
        {
            Debug.Log("Option to set: " + Enum.GetName(typeof(Settings), settingToSet)
                + "\tvalue: " + value);
            _settings.UpdateSetting(settingToSet, value);
            _settingsToChange.Add(settingToSet);
        }
        
        public void ApplySettings()
        {
            if(_settingsToChange.Any())
            {                
                _settingsExecutor.UpdateSettings(_settings, _settingsToChange);
                _settingsToChange.Clear();
            }
        }
        
        public void SaveSettings()
        {
            _dataStorage.SaveToDisk(_settings.Data, _gameFolders.SettingsFolder, currentName);
        }
        
        public void LoadSettings()
        {
            var settingsDataObject = _dataStorage.GetDataObjectFromFile<SettingsData>(_gameFolders.SettingsFolder, currentName);
            _settings = new SettingsContainer(settingsDataObject);
            _settingsToChange.UnionWith(Enum.GetValues(typeof(Settings)).Cast<Settings>());
        }
        
        private void MakeSureSettingsFolderIsPopulated()
        {            
            if(!_dataStorage.FileExists(_gameFolders.SettingsFolder, defaultsName) || _dataStorage.GetDataObjectFromFile<SettingsData>(_gameFolders.SettingsFolder, defaultsName) == null)
            {    
                Debug.LogWarning("No default settings file found at |" + Path.Combine(_gameFolders.SettingsFolder, defaultsName) + "| , creating from default at streamingAssets");
                createDefaultSettingsFile();     
            }
            if(!_dataStorage.FileExists(_gameFolders.SettingsFolder, currentName) || _dataStorage.GetDataObjectFromFile<SettingsData>(_gameFolders.SettingsFolder, currentName) == null)
            {
                Debug.LogWarning("No settings file found at |" + Path.Combine(_gameFolders.SettingsFolder, currentName) + "| , creating from default");
                createSettingsFileFromDefault();
            }
        }
        
        private void createDefaultSettingsFile()
        {
            var settingsFolderName = new DirectoryInfo(_gameFolders.SettingsFolder).Name;
            _dataStorage.CreateFileFromStreamingAssets<SettingsData>(settingsFolderName, defaultsName, _gameFolders.SettingsFolder, defaultsName);
        }
        
        private void createSettingsFileFromDefault()
        {
            _dataStorage.CopyFileData(_gameFolders.SettingsFolder, defaultsName, _gameFolders.SettingsFolder, currentName);
        }
    }    
}

