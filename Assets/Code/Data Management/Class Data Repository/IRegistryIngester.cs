namespace DataManagement
{
    public interface IRegistryIngester
    {
        public void Register(IConfigurable configurableObject, bool updateThisInstanceOnChanges, bool loadStoredFieldsOnRegistration);
        public void Unregister(IConfigurable instance);
    }
}