using System;
using System.Linq;
using System.Collections.Generic;
using DataAccess.DiskAccess.GameFolders;
using System.IO;

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
            
            refreshNonVolatileData();            
            if(updateRegistryOnConstruction)
                UpdateCurrentDataWithNonVolatile();
        } 
        
        public void UpdateCurrentDataWithNonVolatile()
        {      
            try  
            {
                refreshNonVolatileData();
                _registry.OverrideConfigurables(_nonVolatileConfigurablesData);
                UpdateRegistered();
            }            
            catch(System.IO.FileLoadException)
            {
                Console.WriteLine("No file found to update date from non volatile");
            }
        }
        
        public void SaveToNonVolatile()
        {
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
            try
            {
                _nonVolatileConfigurablesData = _nonVolatileStorage.ReadEntry<Dictionary<string, List<ConfigurableField>>>(_pathToEntry);
                if(_nonVolatileConfigurablesData == null)
                    _nonVolatileConfigurablesData = new Dictionary<string, List<ConfigurableField>>();
            }
            catch
            {
                Console.WriteLine("No entry found to deserialize at: " + _pathToEntry);
            }
        }
    }
}