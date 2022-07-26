using DataAccess.DiskAccess.GameFolders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DataManagement
{
    public class ClassDataRegistryManager : IRegistryManager
    {
        IRegistryBackend _registry;
        INonVolatileStorage _nonVolatileStorage;
        string _pathToEntry;        
        Dictionary<string, List<ConfigurableField>> _nonVolatileConfigurablesData = new Dictionary<string, List<ConfigurableField>>();
        
        public event System.EventHandler OnRegisteredUpdated;
        
        public ClassDataRegistryManager(IRegistryBackend registry, INonVolatileStorage nonVolatileStorage, IGameFolders gameFolders, bool updateRegistryOnConstruction = true)
        {
            if(registry == null)
                throw new ArgumentNullException("No registry instance provided to " + this.GetType().Name); 
            if(nonVolatileStorage == null)
                throw new ArgumentNullException("No volatile storage provided to " + this.GetType().Name); 
            
            _registry = registry;
            _nonVolatileStorage = nonVolatileStorage;
            _pathToEntry = Path.Combine(gameFolders.SaveFolder, _registry.Name);
            
            
            if(!NonVolatileContains(_pathToEntry))
                return;
                     
            refreshNonVolatileData();
            if(updateRegistryOnConstruction)
                SyncRegistryAndNonVolatile();
        } 
        
        bool NonVolatileContains(string entry)
        {
            return _nonVolatileStorage.EntryExists(entry);
        }
        
        public void SyncRegistryAndNonVolatile(SyncPriority prioritisedSource = SyncPriority.OnDuplicateGetFromNonVolitile)
        {                 
            if(!NonVolatileContains(_pathToEntry))
            {
                SaveRegisteredToNonVolatile();
                return;
            }            
            
            refreshNonVolatileData();
            var toOverrideRegisteredClasses = (prioritisedSource == SyncPriority.OnDuplicateGetFromNonVolitile) ? true : false;
            _registry.WriteToRegistry(_nonVolatileConfigurablesData, toOverrideRegisteredClasses);
            UpdateRegistered();
        }
        
        public void SaveRegisteredToNonVolatile()
        {
            Debug.Log("Saving registered data to non volatile storage"); 
            var curentConfigurables = (Dictionary<string, List<ConfigurableField>>)_registry.CurrentConfigurablesData.ToDictionary(group => group.Key, group => group.First());
            _nonVolatileConfigurablesData = curentConfigurables;
            _nonVolatileStorage.WriteEntry(_pathToEntry, _nonVolatileConfigurablesData);
        }
        
        public void UpdateRegistered()
        {            
            foreach(var instance in _registry.ObjectsToUpdateOnChange)
                _registry.UpdateInstanceWithStoredValues(instance);
                
            OnRegisteredUpdated?.Invoke(this, EventArgs.Empty);
        }        
                
        void refreshNonVolatileData()
        {
            Dictionary<string, List<ConfigurableField>> dataFromStorage = null;
            try
            {
                dataFromStorage = _nonVolatileStorage.ReadEntry<Dictionary<string, List<ConfigurableField>>>(_pathToEntry);
            }
            catch(Exception e)
            {
                Debug.Log("No entry found to deserialize at: " + _pathToEntry);
                throw new NoEntryException("No entry found in: " + _nonVolatileStorage.GetType().FullName , e);
            }
            finally
            {                
                if(dataIsValid(dataFromStorage))
                    _nonVolatileConfigurablesData = UpdateToValidMetadata(dataFromStorage);
            }
        }
        
        bool dataIsValid(Dictionary<string, List<ConfigurableField>> data)
        {
            if(data == null)
                return false;
            if(!data.Values.Any())
                return false;
            return data.Values.First().Any();
        }
        
        Dictionary<string, List<ConfigurableField>> UpdateToValidMetadata(Dictionary<string, List<ConfigurableField>> data)
        {
            var dataWithNewMetadata = new Dictionary<string, List<ConfigurableField>>();
            foreach(var entry in data)
                dataWithNewMetadata.Add(entry.Key, updateFields(Type.GetType(entry.Key), entry.Value));
                            
            return dataWithNewMetadata;
        }
        
        List<ConfigurableField> updateFields(Type classType, List<ConfigurableField> fields)
        {
            if(!IsValidClass(classType))
                return fields;
                
            var newFields = new List<ConfigurableField>();
            foreach(var field in fields)
            {
                StoredField newMetadata = GetFieldMetadataFromCode(classType, field.Name);      
                if(newMetadata != null)                    
                    newFields.Add(ConfigurableFieldUtils.ImplantWithMetadata(field, newMetadata.Metadata));
                else
                    newFields.Add(field);
            }            
            return newFields;
        }
        
        bool IsValidClass(Type classType)
        {
            if(classType == null)
                return false;
            return Type.GetType(classType.Name) != null;
        }
        
        StoredField GetFieldMetadataFromCode(Type classType , string fieldName)
        {
            StoredField metadataFromCode = null;
            var storedFieldInfo = classType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var storedPropertyInfo = classType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if(storedFieldInfo != null)
                metadataFromCode = (StoredField)Attribute.GetCustomAttribute(storedFieldInfo, typeof(StoredField));
            else if (storedPropertyInfo != null)
                metadataFromCode = (StoredField)Attribute.GetCustomAttribute(storedPropertyInfo, typeof(StoredField));
            
            return metadataFromCode;
        }
    }
}