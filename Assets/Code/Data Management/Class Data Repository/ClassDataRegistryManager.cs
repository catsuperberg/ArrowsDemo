using DataAccess.DiskAccess.GameFolders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace DataManagement
{
    public class ClassDataRegistryManager : IRegistryManager
    {
        IRegistryBackend _registry;
        INonVolatileStorage _nonVolatileStorage;
        string _pathToEntry;        
        Dictionary<string, List<ConfigurableField>> _nonVolatileConfigurablesData = new Dictionary<string, List<ConfigurableField>>();
        
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
                _registry.UpdateInstanceWithStoredValues(instance.Key);
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
                throw new NoEntryException("No entry found in: " + _nonVolatileStorage.GetType().Name , e);
            }
            finally
            {                
                if(dataIsValid(dataFromStorage))
                    _nonVolatileConfigurablesData = dataFromStorage;
            }
        }
        
        bool dataIsValid(Dictionary<string, List<ConfigurableField>> data)
        {
            return data != null && data.Values.First().Any();
        }
    }
}