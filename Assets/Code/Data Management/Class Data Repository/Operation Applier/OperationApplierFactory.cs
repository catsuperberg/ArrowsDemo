using System;
using System.Numerics;

namespace DataManagement
{
    public static class OperationApplierFactory
    {
        public static IOperationApplier GetApplier(string valueType)
        {
            if(valueType == typeof(int).FullName)
                return new IntApplier();
            if(valueType == typeof(BigInteger).FullName)
                return new BigIntApplier();
            else if(valueType == typeof(float).FullName)
                return new FloatApplier();
            else if(valueType == typeof(string).FullName)
                return new StringApplier();
            else if(valueType == typeof(bool).FullName)
                return new BoolApplier();
            throw new ArgumentException("Factory was requested to create applier for illigal type: " + valueType);
        }        
    }    
}