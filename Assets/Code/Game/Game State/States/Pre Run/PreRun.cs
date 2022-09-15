using Game.Gameplay.Meta;
using Game.Gameplay.Realtime;
using System;
using System.Collections;
using UnityEngine;

using Timer = System.Timers.Timer;

namespace Game.GameState
{    
    public class PreRun : MonoBehaviour, IPreRun
    {
        [SerializeField]
        PreRunUI _UI;
        
        IUpgradeContextNotifier _upgradesNotifier; 
        ISkinContextNotifier _skinNotifier; 
        RunthroughContextManager _contextManager;
        
        private Timer _contextUpdateTimer = new Timer();
        
        public GameObject GameObject {get {return gameObject;}}
        public RunthroughContext CurrentRunthroughContext {get; private set;} = null;
        public event EventHandler OnProceedToNextState;
                
        public void Initialize(IUpgradeContextNotifier upgradesNotifier, ISkinContextNotifier skinNotifier, RunthroughContextManager contextManager)
        {                
            _upgradesNotifier = upgradesNotifier ?? throw new ArgumentNullException(nameof(upgradesNotifier));
            _skinNotifier = skinNotifier ?? throw new ArgumentNullException(nameof(skinNotifier));
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                StartCoroutine(ShowLoadingScreenUntilPlayfieldPresent());});
            if(!_contextManager.ContextReady)
                StartLoading();
        }
        
        IEnumerator ShowLoadingScreenUntilPlayfieldPresent()
        {            
            _UI.SwithchToLoadingScreen(); 
            while(!_contextManager.ContextReady)
                yield return null;
            _UI.SwithchToStartScreen(); 
            StartCoroutine(HookUpEventsOnNextFrame());
        }
        
        IEnumerator HookUpEventsOnNextFrame()
        {        
            int i = 1;    
            while(i-- > 0)
                yield return null;
            _UI.OnStartRunthrough += UIStartButtonPressed;
            _upgradesNotifier.OnNewRunthroughComponents += UserContextUpdated;
            _skinNotifier.OnSelectedProjectileSkin += ProjectileSkinUpdated;
            _skinNotifier.OnSelectedCrossbowSkin += CrossbowSkinUpdated;
        }
        
        void StartLoading()
        { 
            if(!_contextManager.RequestBeingProcessed)
                _contextManager.StartContextUpdate();
        }
        
        void UIStartButtonPressed(object caller, EventArgs args)
        {                
            if(!_contextManager.ContextReady)
                return;
                
            CurrentRunthroughContext = _contextManager.CurrentRunthroughContext;
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
                
        void UserContextUpdated(object caller, EventArgs e)
        {            
            _contextManager.RequestContextUpdate();
        }
        
        void ProjectileSkinUpdated(object caller, EventArgs e)     
        {
            if(_contextManager.CurrentRunthroughContext != null)
                _contextManager.UpdateProjectileToSelected();
        } 
            
        void CrossbowSkinUpdated(object caller, EventArgs e)        
        {
            if(_contextManager.CurrentRunthroughContext != null)
                _contextManager.UpdateCrossbowToSelected();
        } 
        
        void OnDestroy()
        {
            _upgradesNotifier.OnNewRunthroughComponents -= UserContextUpdated;
            _upgradesNotifier = null;
            _skinNotifier.OnSelectedProjectileSkin -= ProjectileSkinUpdated;
            _skinNotifier.OnSelectedCrossbowSkin -= CrossbowSkinUpdated;
            _skinNotifier = null;
            Destroy(_UI);
            _UI = null;          
        }
    }
}