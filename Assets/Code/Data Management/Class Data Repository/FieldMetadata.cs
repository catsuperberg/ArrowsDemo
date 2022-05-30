namespace DataManagement
{
    public class FieldMetadata
    {        
        public readonly string PrettyName;
        public readonly float MinValue;
        public readonly float MaxValue;
        
        public FieldMetadata(string prettyName = "Undefined attribute name",
            float minValue = float.NaN, float maxValue = float.NaN)
        {
            PrettyName = prettyName;
            MinValue = minValue;
            MaxValue = maxValue;            
        }
    }
}