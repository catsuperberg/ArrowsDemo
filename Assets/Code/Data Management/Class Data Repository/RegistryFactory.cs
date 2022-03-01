using DataAccess.DiskAccess.GameFolders;
using System;

namespace DataManagement
{
    public class RegistryFactory
    {
        IGameFolders _gameFolders;
        
        public RegistryFactory(IGameFolders gameFolders)
        {
            if(gameFolders == null)
                throw new ArgumentNullException("No IGameFolders provided to " + this.GetType().Name);
                
            _gameFolders = gameFolders;
        }
        
        public Registry CreateRegistry(string registryName, INonVolatileStorage nonVolatileStorage)
        {            
            var registryBackend = new ClassDataRegistryBackend(registryName);
            
            var manager = new ClassDataRegistryManager(registryBackend, nonVolatileStorage, _gameFolders);
            var accessor = new ClassDataRegistryAccessor(registryBackend);            
            var ingester = new ClassDataRegistryIngester(registryBackend, accessor);   
            
            
            return new Registry(ingester, manager, accessor, accessor);
        }
    }
}