using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataManagement
{
    public struct StringValueTypePair
    {        
        public readonly string Value;
        public readonly string Type;

        public StringValueTypePair(string value, string type)
        {
            Value = value;
            Type = type;
        }
    }
    
    public class ConfigurableField
    {
        public readonly string Name;
        public readonly string Value;
        public readonly string Type;
        public readonly FieldMetadata Metadata;
        
        public ConfigurableField(string name, string value, string type, FieldMetadata metadata)
        {
            Name = name;
            Value = value;
            Type = type;
            Metadata = metadata;
        }
        
        // public ConfigurableField(string name, StringValueTypePair valueTypePair, FieldMetadata metadata)
        // {
        //     Name = name;
        //     Value = valueTypePair.Value;
        //     Type = valueTypePair.Type;
        //     Metadata = metadata;
        // }
    }
}