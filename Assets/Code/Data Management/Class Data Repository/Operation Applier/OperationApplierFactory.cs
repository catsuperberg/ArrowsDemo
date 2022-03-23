using System;

namespace DataManagement
{
    public static class OperationApplierFactory
    {
        public static IOperationApplier GetApplier(string valueType)
        {
            if(valueType == typeof(int).FullName)
                return new IntApplier();
            else if(valueType == typeof(float).FullName)
                return new FloatApplier();
            else if(valueType == typeof(string).FullName)
                return new StringApplier();
            throw new ArgumentException("Factory was requested to create applier for illigal type: " + valueType);
        }        
    }    
}