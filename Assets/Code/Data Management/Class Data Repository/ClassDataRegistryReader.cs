using System;
using System.Linq;

namespace DataManagement
{
    public class ClassDataRegistryReader : IRegistryValueReader
    {
        IRegistryBackend _registry;
        
        public ClassDataRegistryReader(IRegistryBackend registry)
        {
            if(registry == null)
                throw new ArgumentNullException("No registry implimentation provided to " + this.GetType().Name);
            
            _registry = registry;
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