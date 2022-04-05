using DataManagement;
using Game.Gameplay.Meta.Curencies;
using Game.Gameplay.Realtime;
using System;
using System.Numerics;
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
        IRegistryAccessor _userContextAccessor;
        // IRuntimeFactory _runtimeFactory;  
        // IUpdatedNotification _userContextNotifier; 
        
        [Inject]
        // public void Construct(IRuntimeFactory runtimeFactory, [Inject(Id = "userContextNotifier")] IUpdatedNotification userContextNotifier)
        public void Construct(PreRunFactory preRunFactory, RunthroughFactory runthroughFactory, 
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor userContextAccessor)
        {
            if(preRunFactory == null)
                throw new ArgumentNullException("PreRunFactory isn't provided to " + this.GetType().Name);
            if(runthroughFactory == null)
                throw new ArgumentNullException("runthroughFactory isn't provided to " + this.GetType().Name);        
            if(userContextAccessor == null)
                throw new ArgumentNullException("IRegistryAccessor not provided to " + this.GetType().Name);
                            
            _preRunFactory = preRunFactory;
            _runthroughFactory = runthroughFactory;
            _userContextAccessor = userContextAccessor;            
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
        
        public PostRun GetPostRun(RunFinishContext FinishContext)
        {                   
            var coinsString = _userContextAccessor.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.CommonCoins));
            var playerCoins = BigInteger.Parse(coinsString);
            
            var postRunGO = Instantiate(PostRunPrefab);
            var postRun = postRunGO.GetComponent<PostRun>();   
            postRun.Initialize(FinishContext, playerCoins);    
            return postRun;   
        }
        
        public AdState GetAd()
        {
            var adStateGO = Instantiate(AdPrefab);
            var adState = adStateGO.GetComponent<AdState>();   
            return adState;            
        }
        
    }
}