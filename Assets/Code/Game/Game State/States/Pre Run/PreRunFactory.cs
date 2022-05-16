using Game.Gameplay.Realtime;
using DataManagement;
using System;
using UI;
using UnityEngine;
using Zenject;

namespace Game.GameState
{
    public class PreRunFactory
    {
        RunthroughContextManager _contextManager;  
        IUpdatedNotification _userContextNotifier; 
        IRegistryAccessor _userContextAccessor;
        
        public PreRunFactory(RunthroughContextManager contextManager, [Inject(Id = "userContextNotifier")] IUpdatedNotification userContextNotifier,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor)
        {
            if(contextManager == null)
                throw new ArgumentNullException("RunthroughContextManager isn't provided to " + this.GetType().Name);
             if(userContextNotifier == null)
                throw new ArgumentNullException("IUpdatedNotification isn't provided to " + this.GetType().Name);
            if(registryAccessor == null)
                throw new System.Exception("IRegistryAccessor isn't provided to " + this.GetType().Name);
                                             
            _contextManager = contextManager;
            _userContextNotifier = userContextNotifier;
            _userContextAccessor = registryAccessor;               
        }
        
        public IPreRun GetPreRun(GameObject preRunPrefab, bool skipToRun)
        {
            if(skipToRun)
            {
                var script = new GameObject("Pre Run").AddComponent<PreRunSkipToRun>();
                script.Initialize(_contextManager);
                return script;
            }
            
            var preRunGO = GameObject.Instantiate(preRunPrefab);
            var preRun = preRunGO.GetComponent<PreRun>();
            preRun.Initialize(_userContextNotifier, _contextManager);
            var upgradeShop = preRunGO.GetComponentInChildren<UpgradeShop>();
            upgradeShop.Initialize(_userContextAccessor); 
            return preRun;
        }
    }
}