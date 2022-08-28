using System;
using System.Linq;

namespace DataManagement
{
    public class ClassDataRegistryReader : IRegistryValueReader
    {
        IRegistryBackend _registry;
        
        public event EventHandler<RegistryChangeArgs> OnNewData 
        {
            add => _registry.OnNewData += value;
            remove => _registry.OnNewData -= value;
        }
        
        public ClassDataRegistryReader(IRegistryBackend registry)
        {
            if(registry == null)
                throw new ArgumentNullException("No registry implimentation provided to " + this.GetType().Name);                
            
            _registry = registry;
            // _registry.OnNewData += DataInRegistryUpdated;
        }
        
        // void DataInRegistryUpdated(object caller, EventArgs args)
        // {
        //     OnUpdated?.Invoke(this, EventArgs.Empty);
        // }        
        
        public Type GetFieldType(Type classType, string fieldName)
        {
            var field = GetFirstField(classType, fieldName);
            var fieldType = Type.GetType(field.Type);
            if(fieldType == null)
                fieldType = typeof(System.Numerics.BigInteger).Assembly.GetType(field.Type); // HACK Too specific about assembly
            return fieldType;                     
        }
                
        public string GetStoredValue(Type classType, string fieldName)
        {            
            var field = GetFirstField(classType, fieldName);
            return field.Value;
        }
        
        public FieldMetadata GetFieldMetadata(Type classType, string fieldName)
        {
            var field = GetFirstField(classType, fieldName);
            return field.Metadata;
        }
        
        ConfigurableField GetFirstField(Type classType, string fieldName)
        {
             if(!_registry.Configurables.ClassRegistered(classType.FullName))
                throw new NullReferenceException("No registered configurables found for class " + classType.FullName);
            var fields = _registry.Configurables.GetFields(classType.FullName);
            var field = fields.FirstOrDefault(x => x.Name == fieldName);
            if(field == null)
                throw new NullReferenceException("No field found for class in regisrty. Class name: " + classType.FullName + "Field: "  + fieldName);
            return field;
        }
        
        public void UpdateInstanceWithStoredValues(IConfigurable instance)
        {
            _registry.UpdateInstanceWithStoredValues(instance);
        }
    }
}