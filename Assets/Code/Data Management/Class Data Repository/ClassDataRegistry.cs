using System;
using System.Linq;
using System.Collections.Generic;

namespace DataManagement
{
    public class ClassDataRegistryBackend : IRegistryBackend
    {
        public string Name {get; private set;}
        public Lookup<string, List<ConfigurableField>> CurrentConfigurablesData {get {return (Lookup<string, List<ConfigurableField>>)_currentConfigurablesData.ToLookup(p => p.Key, p => p.Value);}}
        public Lookup<IConfigurable, string> ObjectsToUpdateOnChange {get {return (Lookup<IConfigurable, string>)_objectsToUpdateOnChange.ToLookup(p => p.Key, p => p.Value);}}
        public Dictionary<string, List<ConfigurableField>> _currentConfigurablesData {get; private set;} = new Dictionary<string, List<ConfigurableField>>();
        public Dictionary<IConfigurable, string> _objectsToUpdateOnChange {get; private set;} = new Dictionary<IConfigurable, string>();
        
        public ClassDataRegistryBackend(string nameForTheRegistry)
        {
            Name = nameForTheRegistry;
        }
        
        public void UpdateRegisteredField(string className, string fieldName, string fieldValue)
        {
            var indexOfTargetField = _currentConfigurablesData[className].FindIndex(x => x.Name == fieldName);
            if(indexOfTargetField != -1)
            {
                var fieldType = _currentConfigurablesData[className][indexOfTargetField].Type;
                _currentConfigurablesData[className][indexOfTargetField] = new ConfigurableField(fieldName, fieldValue, fieldType);
            }
            else
                throw new NullReferenceException("No field found to update, trying to update: " + fieldName + " in " + className);
            
            if(_objectsToUpdateOnChange.ContainsValue(className))
                UpdateSingleFieldOfAllRegistered(className, fieldName, fieldValue);
        }
        
        public void UpdateInstanceWithStoredValues(IConfigurable instance)
        {
            UpdateObjectsFields(instance, instance.GetType().Name);
        }
        
        public void OverrideConfigurables(Dictionary<string, List<ConfigurableField>> newConfigurables)
        {
            _currentConfigurablesData = newConfigurables;
        }
        
        public void OverrideClassData(string className, List<ConfigurableField> newData)
        {
            if(!_currentConfigurablesData.ContainsKey(className))
                throw new System.Exception("No registered configurables for class: " + className);
            
            _currentConfigurablesData[className] = newData;
        }
        
        public void RegisterNewConfigurablesForClass(Type objectType, List<ConfigurableField> fields)
        {
            if(_currentConfigurablesData.ContainsKey(objectType.Name))
                return;
        
            _currentConfigurablesData.Add(objectType.Name, fields);
        }
        
        public void RegisterInstanceForUpdates(IConfigurable instance, string className)
        {
            _objectsToUpdateOnChange.Add(instance, className);
        }
        
        public void UnregisterInstance(IConfigurable instance)
        {
            _objectsToUpdateOnChange.Remove(instance);
        }
        
        void UpdateSingleFieldOfAllRegistered(string className, string fieldName, string fieldValue)
        {
            var objectsToUpdate = _objectsToUpdateOnChange.Where(x => x.Value == className).Select(x => x.Key);
            foreach(var instance in objectsToUpdate)
                TryWritingField(instance, fieldName, fieldValue);
        }
        
        void UpdateAllRegisteredOfClass(string className)
        {
            var objectsToUpdate = _objectsToUpdateOnChange.Where(x => x.Value == className).Select(x => x.Key);
            foreach(var instance in objectsToUpdate)
                UpdateObjectsFields(instance, className);
        }
        
        void UpdateObjectsFields(IConfigurable instanceToUpdate, string className)
        {
            if(!_currentConfigurablesData.ContainsKey(className))
                throw new System.Exception("No registered configurables for class: " + className);
                
            var configurables = _currentConfigurablesData[className];
            TryWritingFields(instanceToUpdate, configurables);                
        }
                
        void TryWritingField(IConfigurable objectToUpdate, string fieldName, string fieldValue)
        {
            try
            {
                objectToUpdate.UpdateField(fieldName, fieldValue);
            }
            catch (Exception)
            {
                Console.WriteLine("there's no such field as: " + fieldName + " in " + objectToUpdate.GetType().Name);
            }
        }
        
        void TryWritingFields(IConfigurable objectToUpdate, List<ConfigurableField> fields)
        {
            var fieldData = new List<(string fieldName, string fieldValue)>();
            foreach(var configurable in fields)
                fieldData.Add((configurable.Name, configurable.Value));
            try
            {
                objectToUpdate.UpdateFields(fieldData);
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't apply all fields to the instance: " + objectToUpdate);
            }
        }
    }
}