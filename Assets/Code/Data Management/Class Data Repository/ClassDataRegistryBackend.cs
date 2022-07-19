using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace DataManagement
{
    public class ClassDataRegistryBackend : IRegistryBackend
    {
        public string Name {get; private set;}
        public Lookup<string, List<ConfigurableField>> CurrentConfigurablesData {get => (Lookup<string, List<ConfigurableField>>)_currentConfigurablesData.ToLookup(p => p.Key, p => p.Value);}
        public Lookup<IConfigurable, string> ObjectsToUpdateOnChange {get => (Lookup<IConfigurable, string>)_objectsToUpdateOnChange.ToLookup(p => p.Key, p => p.Value);}
        public Dictionary<string, List<ConfigurableField>> _currentConfigurablesData {get; private set;} = new Dictionary<string, List<ConfigurableField>>();
        public Dictionary<IConfigurable, string> _objectsToUpdateOnChange {get; private set;} = new Dictionary<IConfigurable, string>();
                
        public event EventHandler OnUpdated;
        
        void NotifyAboutDataUpdate()
        {
            OnUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        public ClassDataRegistryBackend(string nameForTheRegistry)
        {
            Name = nameForTheRegistry;
        }
        
        public void UpdateRegisteredField(string className, string fieldName, string fieldValue)
        {
            var dataUpdated = false;
            var indexOfTargetField = _currentConfigurablesData[className].FindIndex(x => x.Name == fieldName);
            if(indexOfTargetField != -1)
            {
                var currentField = _currentConfigurablesData[className][indexOfTargetField]; 
                _currentConfigurablesData[className][indexOfTargetField] = new ConfigurableField(fieldName, fieldValue, currentField.Type, currentField.Metadata);
                dataUpdated = true;
            }
            else
                throw new NoFieldException("No field found to update, trying to update: " + fieldName + " in " + className);
            
            if(_objectsToUpdateOnChange.ContainsValue(className))
                UpdateSingleFieldOfAllRegistered(className, fieldName, fieldValue);
            
            if(dataUpdated)
                NotifyAboutDataUpdate();
        }
        
        public void UpdateInstanceWithStoredValues(IConfigurable instance)
        {
            UpdateObjectsFields(instance, instance.GetType().FullName);
        }
        
        public void WriteToRegistry(Dictionary<string, List<ConfigurableField>> sourceConfigurableClasses, bool overrideOnPresent)
        {
            if(!sourceConfigurableClasses.Any())
                return;
            foreach(var newClassConfigurables in sourceConfigurableClasses)
            {
                if(!_currentConfigurablesData.ContainsKey(newClassConfigurables.Key))
                    _currentConfigurablesData.Add(newClassConfigurables.Key, newClassConfigurables.Value);
                else if (overrideOnPresent)
                    OverridePresentFields(newClassConfigurables);
                    // _currentConfigurablesData[classConfigurables.Key] = classConfigurables.Value;                    
            }
            NotifyAboutDataUpdate();
        }
        
        void OverridePresentFields(KeyValuePair<string, List<ConfigurableField>> newConfigurables)
        {
            var currentFields = _currentConfigurablesData[newConfigurables.Key];
            foreach(var field in newConfigurables.Value)
                currentFields[currentFields.FindIndex(instance => instance.Name == field.Name)] = field;
            _currentConfigurablesData[newConfigurables.Key] = currentFields;
        }
        
        public void OverrideConfigurables(Dictionary<string, List<ConfigurableField>> newConfigurables)
        {
            _currentConfigurablesData = newConfigurables;
            NotifyAboutDataUpdate();
        }
        
        public void OverrideClassData(string className, List<ConfigurableField> newData)
        {
            if(!_currentConfigurablesData.ContainsKey(className))
                throw new NoConfigurablesException("No registered configurables for class: " + className);
            
            _currentConfigurablesData[className] = newData;
            NotifyAboutDataUpdate();
        }
        
        public void RegisterNewConfigurablesForClass(Type objectType, List<ConfigurableField> fields)
        {
            if(_currentConfigurablesData.ContainsKey(objectType.FullName))
                return;
        
            _currentConfigurablesData.Add(objectType.FullName, fields);
            NotifyAboutDataUpdate();
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
            clearDeletedRegistrations();
            
            var objectsToUpdate = _objectsToUpdateOnChange.Where(x => x.Value == className).Select(x => x.Key);
            foreach(var instance in objectsToUpdate)
                TryWritingField(instance, fieldName, fieldValue);
        }
        
        void clearDeletedRegistrations()
        {
            var dictionarryWithoutEmptyInstances = _objectsToUpdateOnChange
                .Where(f => f.Key != null)
                .ToDictionary(x => x.Key, x => x.Value);
                
            _objectsToUpdateOnChange = dictionarryWithoutEmptyInstances;  
        }
        
        public void UpdateAllRegisteredOfClass(string className)
        {
            var objectsToUpdate = _objectsToUpdateOnChange.Where(x => x.Value == className).Select(x => x.Key);
            foreach(var instance in objectsToUpdate)
                UpdateObjectsFields(instance, className);
        }
        
        void UpdateObjectsFields(IConfigurable instanceToUpdate, string className)
        {
            if(!_currentConfigurablesData.ContainsKey(className))
                throw new NoConfigurablesException("No registered configurables for class: " + className);
                
            var configurables = _currentConfigurablesData[className];
            TryWritingFields(instanceToUpdate, configurables);                
        }
                
        void TryWritingField(IConfigurable objectToUpdate, string fieldName, string fieldValue)
        {
            try
            {
                objectToUpdate.UpdateField(fieldName, fieldValue);
            }
            catch (MissingFieldException)
            {
                Debug.Log("there's no such field as: " + fieldName + " in " + objectToUpdate.GetType().FullName);
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
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.Log("Couldn't apply all fields to the instance: " + objectToUpdate);
                Debug.Log("fields:");
                foreach (var field in fields)
                {
                    Debug.Log(field.Name + " " + field.Value);
                }
            }
        }
    }
}