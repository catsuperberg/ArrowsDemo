namespace DataManagement
{
    public enum SyncPriority
    {
        OnDuplicateGetFromRegistry,
        OnDuplicateGetFromNonVolitile
    }
    
    public interface IRegistryManager
    {        
        public event System.EventHandler OnRegisteredUpdated;
        
        public void SyncRegistryAndNonVolatile(SyncPriority prioritisedSource = SyncPriority.OnDuplicateGetFromNonVolitile);
        public void SaveRegisteredToNonVolatile();
        public void UpdateRegistered();
    }
}