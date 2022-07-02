using System;
using System.Collections.Generic;

namespace Utils
{
    public static class TypeUtils
    {         
        public static bool IsGenericList(Type type) =>
            type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));   
            
        public static bool IsGenericList(string typeString)
        {
            Type type = Type.GetType(typeString);
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));   
        }
    }
}