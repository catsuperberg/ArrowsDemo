using DataManagement;
using System;
using UnityEngine;

namespace Game.Gameplay.Meta
{
    public class SettingsRegistryManager : IUpdatedNotification
    {
        UserContext _context;        
        IRegistryManager _registryManager;    
        IRegistryValueReader _registryReader;       
        
        public event EventHandler OnUpdated;
        
        public SettingsRegistryManager(IRegistryIngester registryIngester, IRegistryManager registryManager, IRegistryValueReader registryReader)
        {            
            if(registryIngester == null)
                throw new ArgumentNullException("No IRegistryIngester provided to class" + this.GetType().Name);
            if(registryManager == null)
                throw new ArgumentNullException("No IRegistryManager provided to class" + this.GetType().Name);
            if(registryReader == null)
                throw new ArgumentNullException( "No IRegistryValueReader provided to class" + this.GetType().Name);
                                        
            _registryManager = registryManager;
            _registryReader = registryReader;
            _registryManager.SyncRegistryAndNonVolatile();   
            _registryManager.UpdateRegistered();  
            
            _registryReader.OnUpdated += DataUpdated;    
        }      
        
        void DataUpdated(object sender, EventArgs e)
        {
            Debug.LogWarning("Settings changed");
            _registryManager.SaveRegisteredToNonVolatile();
            OnUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}