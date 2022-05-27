using System;
using System.Linq;
using System.Collections.Generic;

namespace DataManagement
{
    public class ClassDataRegistryAccessor : IRegistryAccessor, IRegistryValueReader
    {
        IRegistryBackend _registry;
        
        public event EventHandler OnUpdated;
        
        public ClassDataRegistryAccessor(IRegistryBackend registry)
        {
            if(registry == null)
                throw new ArgumentNullException("No registry implimentation provided to " + this.GetType().FullName);
            
            _registry = registry;
            _registry.OnUpdated += DataInRegistryUpdated;
        }     
        
        void DataInRegistryUpdated(object caller, EventArgs args)
        {
            OnUpdated?.Invoke(this, EventArgs.Empty);
        }   
        
        public List<Type> GetRegisteredClasses()
        {
            var classNames = _registry.CurrentConfigurablesData.Select(entry => entry.Key).ToList();
            var classTypes = classNames.Select(entry => Type.GetType(entry)).ToList();
            return classTypes;
        }
        
        public List<string> GetRegisteredFields(Type classType)
        {
            if(!_registry.CurrentConfigurablesData.Contains(classType.FullName))
                throw new NoConfigurablesException("No registered configurables for class: " + classType.FullName);
                
            var fields = _registry.CurrentConfigurablesData[classType.FullName].First();
            var fieldNames = fields.Select(x => x.Name).ToList();
            return fieldNames;
        }  
                                
        public void ResetRegisteredFieldsToDefault(Type classType)
        {
            object tempInstance = null;
            try
            {
                tempInstance = Activator.CreateInstance(classType);                
            }
            catch (MissingMethodException e)
            {
                throw new NotImplementedException(classType + " doesn't have default constructor and thus can't be reset to default values\n" + e);
            }
            finally
            {
                var defaultConfigurables = ConfigurableFieldUtils.GetInstanceConfigurablesWithCurrentValues((IConfigurable)tempInstance, classType);
                _registry.OverrideClassData(classType.FullName, defaultConfigurables);
                _registry.UpdateAllRegisteredOfClass(classType.FullName);
                tempInstance = null;                
            }
        }
        
        public void ApplyOperationOnRegisteredField(Type classType, string fieldName, OperationType operation, string fieldIncrement)
        {
            var valueInRegistry = GetStoredValue(classType, fieldName);
            var fields = _registry.CurrentConfigurablesData[classType.FullName].First();
            var field = fields.FirstOrDefault(x => x.Name == fieldName);
            var fieldType = field.Type;            
            var operationApplier = OperationApplierFactory.GetApplier(fieldType);
            var newValue = operationApplier.GetResultOfOperation(valueInRegistry, fieldIncrement, operation);
            _registry.UpdateRegisteredField(classType.FullName, fieldName, newValue);
        }
                
        public string GetStoredValue(Type classType, string fieldName)
        {
            if(!_registry.CurrentConfigurablesData.Contains(classType.FullName))
                throw new NoConfigurablesException("No registered configurables found for class " + classType.FullName);
            var fields = _registry.CurrentConfigurablesData[classType.FullName].First();
            var field = fields.FirstOrDefault(x => x.Name == fieldName);
            if(field == null)
                throw new NoFieldException("No field found for class in regisrty. Class name: " + classType.FullName + "Field: "  + fieldName);
            return field.Value;
        }
        
        public void UpdateInstanceWithStoredValues(IConfigurable instance)
        {
            _registry.UpdateInstanceWithStoredValues(instance);
        }
    }
}