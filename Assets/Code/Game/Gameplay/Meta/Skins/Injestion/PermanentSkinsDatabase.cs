using AssetScripts.AssetCreation;
using DataAccess.DiskAccess.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Game.Gameplay.Meta.Skins
{
    [Serializable]
    public class PermanentSkinsDatabase<T> : ISkinDatabase<T>, ISkinDatabaseReader<T> where T : ISkinData<T>, ISkinData
    {        
        public IList<T> Skins {get => _skins.AsReadOnly();}
        public string PathToDatabase {get => _pathToDatabase;}
        public readonly string _pathToDatabase;
        [SerializeField]
        List<T> _skins;
        
        public PermanentSkinsDatabase(string jsonPath)
        {
            _pathToDatabase = jsonPath;
            _skins = LoadFromFile() ?? new List<T>();
        }
        
        List<T> LoadFromFile() => JsonFile.LoadFromResources<List<T>>(_pathToDatabase);
        
        public void AddSkinsUniqueByName(List<T> skinsData)
        {
            var namesAlreadyInList = _skins.Select(entry => entry.Name);
            var uniqueSkins = skinsData.Where(entry => !namesAlreadyInList.Contains(entry.Name));  
            _skins.AddRange(uniqueSkins);
        }
        
        public void SetSkinsDataKeepOldPropertiesOnNull(List<T> skinsData)
            => skinsData.ForEach(AddToListOrUpdateCurrent);
            
        public bool AlreadyInDatabase(string skinName)
            => _skins.Any(entry => entry.Name == skinName);
        
        void AddToListOrUpdateCurrent(T data)
        {
            var index = _skins.FindIndex(entry => entry.Name == data.Name);
            if(index != -1)
                _skins[index] = HasNullProperties(data) ? _skins[index].GetNewWithUpdatedValues(data) : data;
            else
                AddNewEntry(data);
                
        }
        
        void AddNewEntry(T data)
        {
            if(HasNullProperties(data))
            {
                data = data.EnrichWithDefaultValues();
                if(HasNullProperties(data))
                    throw new ArgumentNullException("New entry in database can't have null fields after enriching with defaults");
            }
            _skins.Add(data);
        }
        
        bool HasNullProperties(T data)
            => data.GetType().GetProperties().Select(entry => entry.GetValue(data)).Any(value => value == null);
                
        public void SaveToPermanent()
        {
            JsonFile.SaveAsJson(_skins, _pathToDatabase);
            #if UNITY_EDITOR
                if(_pathToDatabase.Contains("Resources"))
                    AssetDatabase.Refresh();
            #endif
        }
    }
}