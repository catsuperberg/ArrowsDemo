using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DataManagement
{
    public static class ConfigurableFieldUtils
    {
        public static List<ConfigurableField> GetInstanceConfigurablesWithCurrentValues(IConfigurable objectReference, Type classType)
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
        
        public static List<ConfigurableField> InjectValues(List<ConfigurableField> recievingChangebles, List<ConfigurableField> sourceChangebles)
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