
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
        
        IUpdatedNotification _userContextNotifier; 
        RunthroughContextManager _contextManager;
        
        private Timer _contextUpdateTimer = new Timer();
        
        public GameObject GameObject {get {return gameObject;}}
        public RunthroughContext CurrentRunthroughContext {get; private set;} = null;
        public event EventHandler OnProceedToNextState;
                
        public void Initialize(IUpdatedNotification userContextNotifier, RunthroughContextManager contextManager)
        {
             if(userContextNotifier == null)
                throw new ArgumentNullException("IUpdatedNotification isn't provided to " + this.GetType().Name);
             if(contextManager == null)
                throw new ArgumentNullException("RunthroughContextManager isn't provided to " + this.GetType().Name);
                
            _userContextNotifier = userContextNotifier;
            _contextManager = contextManager;
            
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
            _userContextNotifier.OnUpdated += UserContextUpdated;
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
        
        void OnDestroy()
        {
            _userContextNotifier.OnUpdated -= UserContextUpdated;
            _userContextNotifier = null;
            Destroy(_UI);
            _UI = null;
            _userContextNotifier = null;            
        }
    }
}