using DataManagement;
using System;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Meta
{
    public class UserContextManager : IUpdatedNotification
    {
        UserContext _context;        
        IRegistryManager _registryManager;        
        
        public event EventHandler OnUpdated;
        
        public UserContextManager([Inject(Id = "userRegistryManager")]IRegistryManager registryManager, UserContext context)
        {            
            if(context == null)
                throw new ArgumentNullException("No UserContext provided to class" + this.GetType().Name);
            if(registryManager == null)
                throw new ArgumentNullException("No IRegistryManager provided to class" + this.GetType().Name);
                                        
            _registryManager = registryManager;
            _context = context;
            _context.Upgrades.OnUpdated += DataUpdated;
            _context.Curencies.OnUpdated += DataUpdated;
            _context.ProjectileSkins.OnUpdated += DataUpdated;
            _registryManager.SyncRegistryAndNonVolatile();   
            _registryManager.UpdateRegistered();       
        }      
        
        void DataUpdated(object sender, EventArgs e)
        {
            Debug.Log("User data changed by: " + sender.GetType() + " hash: " + sender.GetHashCode());
            _registryManager.SaveRegisteredToNonVolatile();
            OnUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}