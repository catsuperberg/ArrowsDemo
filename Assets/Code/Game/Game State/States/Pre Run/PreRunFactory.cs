using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents;
using DataManagement;
using System;
using UI;
using UnityEngine;
using Zenject;

namespace Game.GameState
{
    public class PreRunFactory
    {
        IRuntimeFactory _runtimeFactory;  
        IUpdatedNotification _userContextNotifier; 
        IRegistryAccessor _userContextAccessor;
        
        public PreRunFactory(IRuntimeFactory runtimeFactory, [Inject(Id = "userContextNotifier")] IUpdatedNotification userContextNotifier,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor)
        {
            if(runtimeFactory == null)
                throw new ArgumentNullException("IRuntimeFactory isn't provided to " + this.GetType().Name);
             if(userContextNotifier == null)
                throw new ArgumentNullException("IUpdatedNotification isn't provided to " + this.GetType().Name);
            if(registryAccessor == null)
                throw new System.Exception("IRegistryAccessor isn't provided to " + this.GetType().Name);
                                             
            _runtimeFactory = runtimeFactory;
            _userContextNotifier = userContextNotifier;
            _userContextAccessor = registryAccessor;               
        }
        
        public PreRun GetPreRun(GameObject preRunPrefab)
        {
            var preRunGO = GameObject.Instantiate(preRunPrefab);
            var preRun = preRunGO.GetComponent<PreRun>();
            preRun.Initialize(_runtimeFactory, _userContextNotifier);
            var upgradeShop = preRunGO.GetComponentInChildren<UpgradeShop>();
            upgradeShop.Initialize(_userContextAccessor); 
            return preRun;
        }
    }
}