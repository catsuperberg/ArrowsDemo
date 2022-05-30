namespace DataManagement
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class StoredField : System.Attribute
    {
        public readonly FieldMetadata Metadata;
        
        public StoredField(string prettyName = "Undefined attribute name",
            float minValue = float.NaN, float maxValue = float.NaN)
        {
            Metadata = new FieldMetadata(prettyName, minValue, maxValue);
        }
    }
}
