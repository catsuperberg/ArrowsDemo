using System;
using System.Linq;
using System.Collections.Generic;

namespace DataManagement
{
    public class ClassDataRegistryAccessor : IRegistryAccessor, IRegistryValueReader
    {
        IRegistryBackend _registry;
        
        public ClassDataRegistryAccessor(IRegistryBackend registry)
        {
            if(registry == null)
                throw new ArgumentNullException("No registry implimentation provided to " + this.GetType().Name);
            
            _registry = registry;
        }        
        
        public List<string> GetRegisteredFields(Type classType)
        {
            if(!_registry.CurrentConfigurablesData.Contains(classType.Name))
                throw new System.Exception("No registered configurables for class: " + classType.Name);
                
            var fields = _registry.CurrentConfigurablesData[classType.Name].First();
            var fieldNames = fields.Select(x => x.Name).ToList();
            return fieldNames;
        }  
        
        public void ApplyOperationOnRegisteredField(Type classType, string fieldName, OperationType operation, string fieldIncrement)
        {
            var valueInRegistry = GetStoredValue(classType, fieldName);
            var fields = _registry.CurrentConfigurablesData[classType.Name].First();
            var field = fields.FirstOrDefault(x => x.Name == fieldName);
            var fieldType = field.Type;            
            var operationApplier = OperationApplierFactory.GetApplier(fieldType);
            var newValue = operationApplier.GetResultOfOperation(valueInRegistry, fieldIncrement, operation);
            _registry.UpdateRegisteredField(classType.Name, fieldName, newValue);
        }
                
        public string GetStoredValue(Type classType, string fieldName)
        {
            if(!_registry.CurrentConfigurablesData.Contains(classType.Name))
                throw new NullReferenceException("No registered configurables found for class " + classType.Name);
            var fields = _registry.CurrentConfigurablesData[classType.Name].First();
            var field = fields.FirstOrDefault(x => x.Name == fieldName);
            if(field == null)
                throw new NullReferenceException("No field found for class in regisrty. Class name: " + classType.Name + "Field: "  + fieldName);
            return field.Value;
        }
        
        public void UpdateInstanceWithStoredValues(IConfigurable instance)
        {
            _registry.UpdateInstanceWithStoredValues(instance);
        }
    }
}