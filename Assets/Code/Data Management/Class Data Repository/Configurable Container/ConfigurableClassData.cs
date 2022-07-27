using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataManagement
{
    public class ConfigurableClassData
    {
        public readonly string ClassName;
        public IList<ConfigurableField> Fields {get => _fields?.AsReadOnly();}
        List<ConfigurableField> _fields;

        public ConfigurableClassData(string className, List<ConfigurableField> fields)
        {
            ClassName = className;
            _fields = fields;
        }

    }
}