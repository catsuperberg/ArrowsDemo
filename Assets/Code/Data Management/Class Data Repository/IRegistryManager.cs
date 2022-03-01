namespace DataManagement
{
    public interface IRegistryManager
    {
        public void UpdateCurrentDataWithNonVolatile();
        public void SaveToNonVolatile();
        public void UpdateRegistered();
    }
}