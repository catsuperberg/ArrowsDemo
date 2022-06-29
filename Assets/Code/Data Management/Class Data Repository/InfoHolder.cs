using System;
using System.Reflection;

namespace DataManagement
{
    public interface InfoHolder
    {
        object  Value {get;}
        Type Type {get;}
    }

    public class FieldInfoHolder : InfoHolder
    {        
        public object Value {get; private set;}
        public Type Type {get; private set;}

        public FieldInfoHolder(FieldInfo field, object instance)
        {
            Value = field.GetValue(instance);
            Type = field.FieldType;
        }
    }
    
    public class PropertyInfoHolder : InfoHolder
    {        
        public object Value {get; private set;}
        public Type Type {get; private set;}

        public PropertyInfoHolder(PropertyInfo property, object instance)
        {
            Value = property.GetValue(instance);
            Type = property.PropertyType;
        }
    }
}