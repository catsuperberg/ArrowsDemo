using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace DataManagement
{
    public class ConfigurableClassData
    {
        public readonly string ClassName;
        public IList<ConfigurableField> Fields {get => new ReadOnlyCollection<ConfigurableField>(_fields);}
        IList<ConfigurableField> _fields;

        public ConfigurableClassData(string className, IList<ConfigurableField> fields)
        {
            ClassName = className;
            _fields = fields != null ? fields : new List<ConfigurableField>();
        }

    }
}