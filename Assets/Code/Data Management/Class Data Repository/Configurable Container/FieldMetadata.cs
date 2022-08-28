using System.Collections.Generic;

namespace DataManagement
{
    public class FieldMetadata
    {        
        public readonly string PrettyName;
        public IList<string> ValidOptions {get {return (_validOptions != null) ? _validOptions.AsReadOnly() : null;}}
        List<string> _validOptions;
        public readonly float MinValue;
        public readonly float MaxValue;        
        
        public FieldMetadata(string prettyName = "Undefined attribute name",
            float minValue = float.NaN, float maxValue = float.NaN, List<string> validOptions = null)
        {
            PrettyName = prettyName;
            MinValue = minValue;
            MaxValue = maxValue;
            _validOptions = validOptions;            
        }
    }
}