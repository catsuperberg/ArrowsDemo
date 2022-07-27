using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace DataManagement
{
    public class ClassDataRegistryBackend : IRegistryBackend
    {
        public string Name {get; private set;}
        public IList<IConfigurable> ObjectsToUpdateOnChange {get => _objectsToUpdateOnChange.AsReadOnly();}        
        public IConfigurableCollectionReader Configurables {get => _configurables as IConfigurableCollectionReader;}
        
        public ConfigurablesCollection _configurables {get; private set;} = new ConfigurablesCollection();
        List<IConfigurable> _objectsToUpdateOnChange = new List<IConfigurable>();
                
        public event EventHandler OnUpdated;
        
        void NotifyAboutDataUpdate(object caller, RegistryChangeArgs args)
        {
            OnUpdated?.Invoke(this, EventArgs.Empty);
            UpgradeInstances(args.ClassName);
        }
        
        void UpgradeInstances(string classToUpdate)
        {
            if(_objectsToUpdateOnChange.Any(entry => entry.GetType().FullName == classToUpdate))
                foreach(var field in _configurables.GetFields(classToUpdate))
                    UpdateSingleFieldOfAllRegistered(classToUpdate, field.Name, field.Value);
        }
        
        public ClassDataRegistryBackend(string nameForTheRegistry)
        {
            Name = nameForTheRegistry;
            _configurables.OnChanges += NotifyAboutDataUpdate;
        }
        
        public void UpdateRegisteredField(string className, ConfigurableField field)
            => _configurables.SetRegisteredField(className, field);
        
        public void UpdateInstanceWithStoredValues(IConfigurable instance)
            => UpdateObjectsFields(instance, instance.GetType().FullName);
        
        public void WriteToRegistry(List<ConfigurableClassData> configurablesToPush, bool overrideOnPresent)
            => _configurables.PushData(configurablesToPush, overrideOnPresent);
        
        // public void OverrideConfigurables(ConfigurablesCollection newConfigurables)
        //     => _configurables = newConfigurables;
        
        public void OverrideClassData(string className, List<ConfigurableField> newData)
            => _configurables.SetRegisteredFields(className, newData);
        
        public void RegisterClassIfNew(Type objectType, List<ConfigurableField> fields)
        {
            if(_configurables.ClassRegistered(objectType.FullName))
                return;
        
            _configurables.SetRegisteredFields(objectType.FullName, fields);
        }
        
        public void RegisterInstanceForUpdates(IConfigurable instance, string className)
            => _objectsToUpdateOnChange.Add(instance);
        
        public void UnregisterInstance(IConfigurable instance)
            =>_objectsToUpdateOnChange.Remove(instance);
        
        void UpdateSingleFieldOfAllRegistered(string className, string fieldName, string fieldValue)
        {
            clearDeletedRegistrations();
                        
            var objectsToUpdate = _objectsToUpdateOnChange.Where(x => x.GetType().FullName == className);
            foreach(var instance in objectsToUpdate)
                TryWritingField(instance, fieldName, fieldValue);
        }
        
        void clearDeletedRegistrations()
        {
            var listWithoutEmptyInstances = _objectsToUpdateOnChange
                .Where(f => f != null);
                
            _objectsToUpdateOnChange = listWithoutEmptyInstances.ToList();  
        }
        
        public void UpdateAllRegisteredOfClass(string className)
        {
            var objectsToUpdate = _objectsToUpdateOnChange.Where(x => x.GetType().FullName == className);
            foreach(var instance in objectsToUpdate)
                UpdateObjectsFields(instance, className);
        }
        
        void UpdateObjectsFields(IConfigurable instanceToUpdate, string className)
        {
            if(!_configurables.ClassRegistered(className))
                throw new NoConfigurablesException("No registered configurables for class: " + className);
                
            var configurables = _configurables.GetFields(className);
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
        
        void TryWritingFields(IConfigurable objectToUpdate, IList<ConfigurableField> fields)
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