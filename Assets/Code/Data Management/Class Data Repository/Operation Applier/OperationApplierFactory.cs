using System;
using System.Numerics;
using static Utils.TypeUtils;

namespace DataManagement
{
    public static class OperationApplierFactory
    {
        public static IOperationApplier GetApplier(string valueType)
        {
            if(valueType == typeof(int).FullName)
                return new IntApplier();
            else if(valueType == typeof(BigInteger).FullName)
                return new BigIntApplier();
            else if(valueType == typeof(float).FullName)
                return new FloatApplier();
            else if(valueType == typeof(string).FullName)
                return new StringApplier();
            else if(valueType == typeof(bool).FullName)
                return new BoolApplier();
            else if(IsGenericList(valueType))
                return new BoolApplier();
            throw new ArgumentException("Factory was requested to create applier for illigal type: " + valueType);
        }        
    }    
}