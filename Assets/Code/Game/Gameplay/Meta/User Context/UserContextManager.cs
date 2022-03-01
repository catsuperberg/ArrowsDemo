using DataManagement;
using Game.Gameplay.Meta.PassiveIncome;
using Game.Gameplay.Meta.UpgradeSystem;
using System;
using UnityEngine;

namespace Game.Gameplay.Meta
{
    public class UserContextManager
    {
        UserContext _context;        
        IRegistryManager _registryManager;
        
        public UserContextManager(IRegistryIngester registryIngester, IRegistryManager registryManager)
        {            
            if(registryIngester == null)
                throw new ArgumentNullException("No IRegistryIngester provided to class" + this.GetType().Name);
            if(registryManager == null)
                throw new ArgumentNullException("No IRegistryManager provided to class" + this.GetType().Name);
                                        
            _registryManager = registryManager;
            _context = new UserContext(new UpgradeContext(registryIngester), new PassiveInvomceContext());
            _context.Upgrades.OnUpdated += DataUpdated;
            _registryManager.UpdateCurrentDataWithNonVolatile();
        }      
        
        void DataUpdated(object sender, EventArgs e)
        {
            Debug.Log("User data changed");
            _registryManager.SaveToNonVolatile();
        }
    }
}