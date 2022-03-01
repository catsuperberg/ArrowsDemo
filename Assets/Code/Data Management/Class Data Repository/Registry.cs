using System;

namespace DataManagement
{
    public class Registry
    {
        public readonly IRegistryIngester Ingester;
        public readonly IRegistryManager Manager;
        public readonly IRegistryAccessor Accessor;
        public readonly IRegistryValueReader Reader;
        
        public Registry(IRegistryIngester ingester, IRegistryManager manager, IRegistryAccessor accessor,  IRegistryValueReader reader)
        {
            if(ingester == null)
                throw new ArgumentNullException("IRegistryIngester wasn't provided to " + this.GetType().Name);
            if(manager == null)
                throw new ArgumentNullException("IRegistryManager wasn't provided to " + this.GetType().Name);
            if(accessor == null)
                throw new ArgumentNullException("IRegistryAccessor wasn't provided to " + this.GetType().Name);
            if(reader == null)
                throw new ArgumentNullException("IRegistryValueReader wasn't provided to " + this.GetType().Name);
                                
            Ingester = ingester;
            Manager = manager;
            Accessor = accessor;
            Reader = reader;    
        }
    }
}