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
        IRunthroughFactory _runthroughFactory;  
        IUpdatedNotification _userContextNotifier; 
        IRegistryAccessor _userContextAccessor;
        
        public PreRunFactory(IRunthroughFactory runthroughFactory, [Inject(Id = "userContextNotifier")] IUpdatedNotification userContextNotifier,
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor registryAccessor)
        {
            if(runthroughFactory == null)
                throw new ArgumentNullException("IRunthroughFactory isn't provided to " + this.GetType().Name);
             if(userContextNotifier == null)
                throw new ArgumentNullException("IUpdatedNotification isn't provided to " + this.GetType().Name);
            if(registryAccessor == null)
                throw new System.Exception("IRegistryAccessor isn't provided to " + this.GetType().Name);
                                             
            _runthroughFactory = runthroughFactory;
            _userContextNotifier = userContextNotifier;
            _userContextAccessor = registryAccessor;               
        }
        
        public PreRun GetPreRun(GameObject preRunPrefab)
        {
            var preRunGO = GameObject.Instantiate(preRunPrefab);
            var preRun = preRunGO.GetComponent<PreRun>();
            preRun.Initialize(_runthroughFactory, _userContextNotifier);
            var upgradeShop = preRunGO.GetComponentInChildren<UpgradeShop>();
            upgradeShop.Initialize(_userContextAccessor); 
            return preRun;
        }
    }
}