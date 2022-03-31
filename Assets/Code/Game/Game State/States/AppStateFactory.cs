using Game.Gameplay.Realtime;
using System;
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
            
        }
        
        public PreRun GetPreRun()
        {
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
            var postRunGO = Instantiate(PostRunPrefab);
            var postRun = postRunGO.GetComponent<PostRun>();       
            return postRun;   
        }
        
        public Ad GetAd()
        {
            return null;            
        }
        
    }
}