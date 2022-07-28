using System;
using System.Collections.Generic;
using System.Linq;

namespace DataManagement
{
    public interface IConfigurableCollectionReader
    {
        public IList<ConfigurableClassData> RegisteredConfigurables {get;}
        
        public bool ClassRegistered(string className);
        public IList<ConfigurableField> GetFields(string className);
    }
    
    public class ConfigurablesCollection : IConfigurableCollectionReader
    {
        public IList<ConfigurableClassData> RegisteredConfigurables {get => _dataForClasses.AsReadOnly();}
        List<ConfigurableClassData> _dataForClasses = new List<ConfigurableClassData>(); 
        
        public event EventHandler<RegistryChangeArgs> OnChanges;
        
        public ConfigurablesCollection()    {}
        
        public ConfigurablesCollection(List<ConfigurableClassData> initialData)
        {
            _dataForClasses = initialData;
        }
        
        public bool ClassRegistered(string className) => _dataForClasses.Any(configurable => configurable.ClassName == className);
        
        public void SetRegisteredField(string className, ConfigurableField field)
        {
            var configurable = GetOrCreateClassData(className);
            SetFields(configurable, new List<ConfigurableField>{field});
        }
        
        public void SetRegisteredFields(string className, List<ConfigurableField> fields)
        {            
            var configurable = GetOrCreateClassData(className);
            SetFields(configurable, fields);
        }
        
        public void PushData(IList<ConfigurableClassData> newData, bool overrideOnPresent)
        {
            var configurablesToAdd = newData.Where(newConfigurable => !_dataForClasses.Any(configurable => configurable.ClassName == newConfigurable.ClassName));
            var configurablesToUpdate = newData.Except(configurablesToAdd);
            _dataForClasses.AddRange(configurablesToAdd);
            foreach(var configurable in configurablesToUpdate)
            {
                var registeredConfigurable = _dataForClasses.FirstOrDefault(oldConfigurable => oldConfigurable.ClassName == configurable.ClassName);
                if(overrideOnPresent)
                    SetFields(registeredConfigurable, configurable.Fields.ToList());
                else
                    SetOnlyNewFields(registeredConfigurable, configurable.Fields.ToList());
            }
        }
        
        public IList<ConfigurableField> GetFields(string className)
            => _dataForClasses.FirstOrDefault(configurable => configurable.ClassName == className)?.Fields;
            
        ConfigurableClassData GetOrCreateClassData(string className)
        {
            var configurable = _dataForClasses.FirstOrDefault(instance => instance.ClassName == className);
            if(configurable == null)
                return AddNewClass(className);
            return configurable;
        }
        
        ConfigurableClassData AddNewClass(string className, List<ConfigurableField> fields = null)
        {
            var newClassData = new ConfigurableClassData(className, fields);
            _dataForClasses.Add(newClassData);
            return newClassData;
        }
        
        void SetFields(ConfigurableClassData configurable, List<ConfigurableField> newFields)
        {
            var unupdatedFields = configurable?.Fields?.Where(oldField => !newFields.Any(newField => oldField.Name == newField.Name)).ToList();
            if(unupdatedFields != null)
                newFields.AddRange(unupdatedFields);
            _dataForClasses[_dataForClasses.IndexOf(configurable)] = new ConfigurableClassData(configurable.ClassName, newFields);
            
            if(newFields != null && newFields.Any())
                OnChanges?.Invoke(this, new RegistryChangeArgs(configurable.ClassName, newFields.Select(field => field.Name).ToList()));
        }
        
        void SetOnlyNewFields(ConfigurableClassData configurable, List<ConfigurableField> newFields)
        {           
            var presentFields = configurable?.Fields?.ToList();
            if(presentFields == null)
            {
                SetFields(configurable, newFields);
                return;
            }
            
            var fieldsToAdd = newFields.Where(newField => !presentFields.Any(oldField => oldField.Name == newField.Name)).ToList();
            
            _dataForClasses[_dataForClasses.IndexOf(configurable)] = new ConfigurableClassData(configurable.ClassName, presentFields.Concat(fieldsToAdd).ToList());
            
            if(fieldsToAdd != null && fieldsToAdd.Any())
                OnChanges?.Invoke(this, new RegistryChangeArgs(configurable.ClassName, fieldsToAdd.Select(field => field.Name).ToList()));
        }
    }
}