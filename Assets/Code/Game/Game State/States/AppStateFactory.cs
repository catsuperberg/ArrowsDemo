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
        GameObject PostRunFailedPrefab;                
        [SerializeField]
        GameObject AdPrefab;
        
        PreRunFactory _preRunFactory;   
        RunthroughFactory _runthroughFactory;
        IRegistryAccessor _userContextAccessor;
        RunthroughContextManager _contextManager;  
        
        [Inject]
        public void Construct(PreRunFactory preRunFactory, RunthroughFactory runthroughFactory, 
            [Inject(Id = "userRegistryAccessor")] IRegistryAccessor userContextAccessor, RunthroughContextManager contextManager)
        {
            if(preRunFactory == null)
                throw new ArgumentNullException("PreRunFactory isn't provided to " + this.GetType().Name);
            if(runthroughFactory == null)
                throw new ArgumentNullException("runthroughFactory isn't provided to " + this.GetType().Name);        
            if(userContextAccessor == null)
                throw new ArgumentNullException("IRegistryAccessor not provided to " + this.GetType().Name);
            if(contextManager == null)
                throw new ArgumentNullException("RunthroughContextManager isn't provided to " + this.GetType().Name);
                            
            _preRunFactory = preRunFactory;
            _runthroughFactory = runthroughFactory;
            _userContextAccessor = userContextAccessor;  
            _contextManager = contextManager;          
        }
        
        public IPreRun GetPreRun(bool skipToRun)
        {
            var preRun = _preRunFactory.GetPreRun(PreRunPrefab, skipToRun);
            return preRun;
        }
        
        public Runthrough GetRunthrough(RunthroughContext runContext)
        {
            var runthrough = _runthroughFactory.GetRunthrough(RunthroughPrefab, runContext);            
            return runthrough;
        }
        
        public IPostRun GetPostRun(RunFinishContext FinishContext)
        {          
            IPostRun postRun = null;     
            if(!FinishContext.RunFailed)
            {
                var coinsString = _userContextAccessor.GetStoredValue(typeof(CurenciesContext), nameof(CurenciesContext.CommonCoins));
                var playerCoins = BigInteger.Parse(coinsString);
                
                var postRunGO = Instantiate(PostRunPrefab);
                var postRunComponent = postRunGO.GetComponent<PostRun>();   
                postRunComponent.Initialize(FinishContext, playerCoins, _contextManager);  
                postRun = postRunComponent;
            }
            else
            {
                var postRunGO = Instantiate(PostRunFailedPrefab); 
                var postRunComponent = postRunGO.GetComponent<PostRunFailedRun>();   
                postRunComponent.Initialize(_contextManager);  
                postRun = postRunComponent;          
            }
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