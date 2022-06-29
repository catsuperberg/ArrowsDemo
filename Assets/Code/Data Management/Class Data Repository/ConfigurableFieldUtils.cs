using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

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
                  
            foreach(var field in fields)
            {
                var metadata = (StoredField)Attribute.GetCustomAttribute(field, typeof(StoredField));
                var ValueTypePair = GetValueType(new FieldInfoHolder(field, objectReference));
                configurables.Add(new ConfigurableField(field.Name, ValueTypePair.Value, ValueTypePair.Type, metadata.Metadata));
            }
            foreach(var property in properties)
            {
                var metadata = (StoredField)Attribute.GetCustomAttribute(property, typeof(StoredField));
                var ValueTypePair = GetValueType(new PropertyInfoHolder(property, objectReference));
                configurables.Add(new ConfigurableField(property.Name, ValueTypePair.Value, ValueTypePair.Type, metadata.Metadata));
            }
            return configurables;
        }
        
        static StringValueTypePair GetValueType(InfoHolder infoHolder)
        {
            var stringValue = (IsGenericList(infoHolder.Type)) ?
                JsonConvert.SerializeObject(infoHolder.Value, Formatting.Indented) :
                infoHolder.Value.ToString();
            
             return new StringValueTypePair(stringValue, infoHolder.Type.ToString());          
        }
        
        static bool IsGenericList(Type type) =>
            type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
        
        
        public static List<ConfigurableField> InjectValues(List<ConfigurableField> recievingChangebles, List<ConfigurableField> sourceChangebles)
        {
            var configurablesWithUpdatetValues = new List<ConfigurableField>();
            foreach(var field in recievingChangebles)
            {
                string value;
                var providerField = sourceChangebles.FirstOrDefault(x => x.Name == field.Name);
                value = (providerField != null && field.Type == providerField.Type) ? providerField.Value : field.Value;
                configurablesWithUpdatetValues.Add(new ConfigurableField(field.Name, value, field.Type, field.Metadata));
            }
            return configurablesWithUpdatetValues;
        }
        
        public static ConfigurableField ImplantWithMetadata(ConfigurableField sourceField, FieldMetadata newMetadata)
        {
            return new ConfigurableField(sourceField.Name, sourceField.Value, sourceField.Type, newMetadata);
        }
    }
}