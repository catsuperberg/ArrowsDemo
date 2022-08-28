using System;
using System.Linq;
using System.Collections.Generic;

namespace DataManagement
{
    public class ClassDataRegistryAccessor : ClassDataRegistryReader, IRegistryAccessor 
    {
        IRegistryBackend _registry;
        
        public ClassDataRegistryAccessor(IRegistryBackend registry) : base(registry)    
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        } 
        
        public List<Type> GetRegisteredClasses()
        {
            var classNames = _registry.Configurables.RegisteredConfigurables.Select(entry => entry.ClassName).ToList();
            var classTypes = classNames.Select(entry => Type.GetType(entry)).ToList();
            return classTypes;
        }
        
        public List<string> GetRegisteredFields(Type classType)
        {
            if(!_registry.Configurables.ClassRegistered(classType.FullName))
                throw new NoConfigurablesException("No registered configurables for class: " + classType.FullName);
                
            var fields = _registry.Configurables.GetFields(classType.FullName);
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
                var defaultConfigurables = ConfigurableFieldUtils.GetInstanceFieldsWithCurrentValues((IConfigurable)tempInstance, classType);
                _registry.OverrideClassData(classType.FullName, defaultConfigurables);
                _registry.UpdateAllRegisteredOfClass(classType.FullName);
                tempInstance = null;                
            }
        }
        
        public void ApplyOperationOnRegisteredField(Type classType, string fieldName, OperationType operation, string fieldIncrement)
        {
            var valueInRegistry = GetStoredValue(classType, fieldName);
            var fields = _registry.Configurables.GetFields(classType.FullName);
            var field = fields.FirstOrDefault(x => x.Name == fieldName);
            var fieldType = field.Type;            
            var operationApplier = OperationApplierFactory.GetApplier(fieldType);
            var newValue = operationApplier.GetResultOfOperation(valueInRegistry, fieldIncrement, operation);
            var Configurables = new ConfigurableField(field.Name, newValue, field.Type, field.Metadata);
            _registry.UpdateRegisteredField(classType.FullName, Configurables);
        }
    }
}