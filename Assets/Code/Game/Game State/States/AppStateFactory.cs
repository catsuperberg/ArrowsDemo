using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.PlayfieldComponents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.GameState
{    
    public class AppStateFactory : MonoBehaviour, IAppStateFactory
    {             
        [SerializeField]
        GameObject PreRunPrefab;
        [SerializeField]
        GameObject RunthroughPrefab;
        [SerializeField]
        GameObject PostRunPrefab;
        [SerializeField]
        GameObject AdPrefab;
        
        PreRunFactory _preRunFactory;   
        RunthroughFactory _runthroughFactory;
        // IRuntimeFactory _runtimeFactory;  
        // IUpdatedNotification _userContextNotifier; 
        
        [Inject]
        // public void Construct(IRuntimeFactory runtimeFactory, [Inject(Id = "userContextNotifier")] IUpdatedNotification userContextNotifier)
        public void Construct(PreRunFactory preRunFactory, RunthroughFactory runthroughFactory)
        {
            if(preRunFactory == null)
                throw new ArgumentNullException("PreRunFactory isn't provided to " + this.GetType().Name);
            if(runthroughFactory == null)
                throw new ArgumentNullException("runthroughFactory isn't provided to " + this.GetType().Name);
                
            _preRunFactory = preRunFactory;
            _runthroughFactory = runthroughFactory;
            // if(runtimeFactory == null)
            //     throw new ArgumentNullException("IRuntimeFactory isn't provided to " + this.GetType().Name);
            //  if(userContextNotifier == null)
            //     throw new ArgumentNullException("IUpdatedNotification isn't provided to " + this.GetType().Name);
                
            // _runtimeFactory = runtimeFactory;
            // _userContextNotifier = userContextNotifier;
            
        }
        
        public PreRun GetPreRun()
        {
            // var preRunGO = Instantiate(PreRunPrefab);
            // var preRun = preRunGO.GetComponent<PreRun>();
            // preRun.Initialize(_runtimeFactory, _userContextNotifier);
            var preRun = _preRunFactory.GetPreRun(PreRunPrefab);
            return preRun;
        }
        
        public Runthrough GetRunthrough(RunthroughContext runContext)
        {
            var runthrough = _runthroughFactory.GetRunthrough(RunthroughPrefab, runContext);            
            return runthrough;
        }
        
        public PostRun GetPostRun()
        {
            return null;            
        }
        
        public Ad GetAd()
        {
            return null;            
        }
        
    }
}