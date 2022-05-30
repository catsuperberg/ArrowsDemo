namespace DataManagement
{
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
    }
}