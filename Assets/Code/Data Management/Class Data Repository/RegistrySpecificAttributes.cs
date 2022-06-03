using System;
using System.Collections.Generic;
using System.Linq;

namespace DataManagement
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class StoredField : System.Attribute
    {
        public readonly FieldMetadata Metadata;
        
        public StoredField(string prettyName = "Undefined attribute name",
            float minValue = float.NaN, float maxValue = float.NaN, Type OptionGetter = null)
        {         
            IOptionsGetter getter = (OptionGetter != null) ? (IOptionsGetter)Activator.CreateInstance(OptionGetter) : null;            
            var options = (getter != null) ? getter.Options.ToList() : null;
            
            Metadata = new FieldMetadata(prettyName, minValue, maxValue, options);
        }
    }
}
