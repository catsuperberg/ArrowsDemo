using DataManagement;
using System;
using UnityEngine;

namespace Settings
{
    public class SettingsRegistryManager : IUpdatedNotification
    {     
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
            
            _registryReader.OnNewData += DataUpdated;    
        }      
        
        void DataUpdated(object sender, EventArgs e)
        {
            Debug.Log("Settings changed");
            _registryManager.SaveRegisteredToNonVolatile();
            OnUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}