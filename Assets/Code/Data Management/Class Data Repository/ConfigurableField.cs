namespace DataManagement
{
    public class ConfigurableField
    {
        public readonly string Name;
        public readonly string Value;
        public readonly string Type;
        
        public ConfigurableField(string name, string value, string type)
        {
            Name = name;
            Value = value;
            Type = type;
        }
    }
}