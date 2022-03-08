using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
            Debug.Log("Registering instance of class: " + configurableObject.GetType() + " in class data registry");
            var classType = configurableObject.GetType();   
            ActualizeClassFields(configurableObject, classType);         
            if(!_registry.CurrentConfigurablesData.Contains(classType.Name))
                RegisterConfigurablesForClass(configurableObject, classType);    
            if(updateThisInstanceOnChanges)
                _registry.RegisterInstanceForUpdates(configurableObject, classType.Name);
            if(loadStoredFieldsOnRegistration)
                _registryAccessor.UpdateInstanceWithStoredValues(configurableObject);
        }
        
        public void Unregister(IConfigurable instance)
        {
            _registry.UnregisterInstance(instance);
        }
        
        void RegisterConfigurablesForClass(IConfigurable configurableObject, Type classType)
        {
            var defaultConfigurables = GetInstanceConfigurablesWithCurrentValues(configurableObject, classType);
            List<ConfigurableField> configurables;
            if(_registry.CurrentConfigurablesData.Contains(classType.Name))
                configurables = InjectValues(defaultConfigurables, _registry.CurrentConfigurablesData[classType.Name].First());
            else
                configurables = defaultConfigurables;
            _registry.RegisterNewConfigurablesForClass(classType, configurables);
        }
        
        void ActualizeClassFields(IConfigurable configurableObject, Type classType)
        {
            var defaultConfigurables = GetInstanceConfigurablesWithCurrentValues(configurableObject, classType);
            var storedChangebles = _registry.CurrentConfigurablesData[classType.Name].FirstOrDefault();
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
                    fieldToAdd = (fieldFoundInRegestry.Type == field.Type) ? fieldFoundInRegestry : field;   
                else
                    fieldToAdd =field;
                configurablesWithUpdatetValues.Add(fieldToAdd);
            }
            _registry.OverrideClassData(classType.Name, configurablesWithUpdatetValues);
        }
        
        bool temp(FieldInfo info)
        {
            if(Attribute.IsDefined(info, typeof(StoredField), true))
                  Debug.Log("hello");
            return false;
        }
        
        List<ConfigurableField> GetInstanceConfigurablesWithCurrentValues(IConfigurable objectReference, Type classType)
        {   
            var configurables = new List<ConfigurableField>();
            var fields = classType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(field => Attribute.IsDefined(field, typeof(StoredField))); 
            var properties = classType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(field => Attribute.IsDefined(field, typeof(StoredField))); 
                                    
            if(!fields.Any() && !properties.Any())
                throw new NoFieldException("No fields with [StoredField] attribute was found in " + classType);
                
            Debug.Log("Adding fields and properties of a class: " + classType + " to registry");                
            foreach(var field in fields)
                Debug.Log("Field added to registy: " + field.Name + " " + field.GetValue(objectReference).ToString() + " " + field.FieldType.ToString());
            foreach(var property in properties)
                Debug.Log("property added to registy: " + property.Name + " " + property.GetValue(objectReference).ToString() + " " + property.PropertyType.ToString());    
                   
            foreach(var field in fields)
                configurables.Add(new ConfigurableField(field.Name, field.GetValue(objectReference).ToString(), field.FieldType.ToString()));
            foreach(var property in properties)
                configurables.Add(new ConfigurableField(property.Name, property.GetValue(objectReference).ToString(), property.PropertyType.ToString()));
            return configurables;
        }
        
        List<ConfigurableField> InjectValues(List<ConfigurableField> recievingChangebles, List<ConfigurableField> sourceChangebles)
        {
            var configurablesWithUpdatetValues = new List<ConfigurableField>();
            foreach(var field in recievingChangebles)
            {
                string value;
                var providerField = sourceChangebles.FirstOrDefault(x => x.Name == field.Name);
                value = (providerField != null && field.Type == providerField.Type) ? providerField.Value : field.Value;
                configurablesWithUpdatetValues.Add(new ConfigurableField(field.Name, value, field.Type));
            }
            return configurablesWithUpdatetValues;
        }
    }
}