using System;
using System.Collections.Generic;

namespace DataManagement
{
    public interface IRegistryAccessor : IRegistryValueReader
    {
        public List<string> GetRegisteredFields(Type classType);            
        public void ResetRegisteredFieldsToDefault(Type classType);
        public void ApplyOperationOnRegisteredField(Type classType, string fieldName, OperationType operation, string fieldIncrement);
    }
}