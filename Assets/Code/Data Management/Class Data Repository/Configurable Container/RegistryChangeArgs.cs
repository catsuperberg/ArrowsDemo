using System;
using System.Collections.Generic;

namespace DataManagement
{
    public class RegistryChangeArgs : EventArgs
    {
        public readonly string ClassName;
        public readonly List<string> Fields;

        public RegistryChangeArgs(string className, List<string> fields)
        {
            ClassName = className;
            Fields = fields;
        }
    }
}