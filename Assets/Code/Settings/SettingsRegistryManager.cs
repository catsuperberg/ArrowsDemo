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
        
        public SettingsRegistryManager(
            IRegistryManager registryManager, 
            IRegistryValueReader registryReader)
        {
            _registryManager = registryManager ?? throw new ArgumentNullException(nameof(registryManager));
            _registryReader = registryReader ?? throw new ArgumentNullException(nameof(registryReader));
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