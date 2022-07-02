using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DataManagement
{
    public class ClassDataRegistryIngester : IRegistryIngester
    {
        IRegistryBackend _registry;
        IRegistryAccessor _registryAccessor;
        
        public ClassDataRegistryIngester(IRegistryBackend registry, IRegistryAccessor accessor)
        {
            if(registry == null)
                throw new ArgumentNullException("No registry implimentation provided to " + this.GetType().Name);
            if(accessor == null)
                throw new ArgumentNullException("No registry accessor implimentation provided to " + this.GetType().Name);
            
            _registry = registry;
            _registryAccessor = accessor;
        }
        
        public void Register(IConfigurable configurableObject, bool updateThisInstanceOnChanges, bool loadStoredFieldsOnRegistration)
        {
            var classType = configurableObject.GetType();   
            ActualizeClassFields(configurableObject, classType);         
            if(!_registry.CurrentConfigurablesData.Contains(classType.FullName))
                RegisterConfigurablesForClass(configurableObject, classType);    
            if(updateThisInstanceOnChanges)
                _registry.RegisterInstanceForUpdates(configurableObject, classType.FullName);
            if(loadStoredFieldsOnRegistration)
                _registryAccessor.UpdateInstanceWithStoredValues(configurableObject);
        }
        
        public void Unregister(IConfigurable instance)
        {
            _registry.UnregisterInstance(instance);
        }
        
        void RegisterConfigurablesForClass(IConfigurable configurableObject, Type classType)
        {
            var defaultConfigurables = ConfigurableFieldUtils.GetInstanceConfigurablesWithCurrentValues(configurableObject, classType);
            List<ConfigurableField> configurables;
            if(_registry.CurrentConfigurablesData.Contains(classType.FullName))
                configurables = ConfigurableFieldUtils.InjectValues(defaultConfigurables, _registry.CurrentConfigurablesData[classType.FullName].First());
            else
                configurables = defaultConfigurables;
            _registry.RegisterNewConfigurablesForClass(classType, configurables);
        }
        
        void ActualizeClassFields(IConfigurable configurableObject, Type classType)
        {
            var defaultConfigurables = ConfigurableFieldUtils.GetInstanceConfigurablesWithCurrentValues(configurableObject, classType);
            var storedChangebles = _registry.CurrentConfigurablesData[classType.FullName].FirstOrDefault();
            if(storedChangebles == null)
            {
                Debug.Log("No changebles found in storage for: " + classType);
                Debug.Log("Skiping actualizing");
                return;                
            }
                
            var configurablesWithUpdatetValues = new List<ConfigurableField>();
            foreach(var field in defaultConfigurables)
            {
                ConfigurableField fieldToAdd;
                var fieldFoundInRegestry = storedChangebles.FirstOrDefault(x => x.Name == field.Name);
                if(fieldFoundInRegestry != null)
                    fieldToAdd = GetStoredFieldIfValid(fieldFoundInRegestry, field);
                else
                    fieldToAdd = field;
                configurablesWithUpdatetValues.Add(fieldToAdd);
            }
            _registry.OverrideClassData(classType.FullName, configurablesWithUpdatetValues);
        }
        
        ConfigurableField GetStoredFieldIfValid(ConfigurableField storedField, ConfigurableField ingestedField)
        {
            var field = (storedField.Type == ingestedField.Type) ? 
                        GetStoredFieldWithValidMetadata(storedField, ingestedField) : 
                        ingestedField;   
            return field;
        }
        
        ConfigurableField GetStoredFieldWithValidMetadata(ConfigurableField storedField, ConfigurableField ingestedField)
        {
            return ConfigurableFieldUtils.ImplantWithMetadata(storedField, ingestedField.Metadata);
        }
    }
}