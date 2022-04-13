using System;

namespace DataManagement
{
    public interface IRegistryValueReader : IUpdatedNotification
    {        
        public string GetStoredValue(Type classType, string fieldName);
        public void UpdateInstanceWithStoredValues(IConfigurable instance);  
    }
}