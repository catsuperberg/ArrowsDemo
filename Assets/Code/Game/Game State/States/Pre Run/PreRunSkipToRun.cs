using Game.Gameplay.Realtime;
using System;
using System.Collections;
using UnityEngine;


namespace Game.GameState
{    
    public class PreRunSkipToRun : MonoBehaviour, IPreRun
    {
        RunthroughContextManager _contextManager;
        
        public GameObject GameObject {get {return gameObject;}}
        public RunthroughContext CurrentRunthroughContext {get; private set;} = null;
        public event EventHandler OnProceedToNextState;
                
        public void Initialize(RunthroughContextManager contextManager)
        {
             if(contextManager == null)
                throw new ArgumentNullException("Runt   hroughContextManager isn't provided to " + this.GetType().Name);
                
            _contextManager = contextManager;
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                StartCoroutine(ProceedToNextStateWhenContextReady());});
            if(!_contextManager.ContextReady)
                StartLoading();
        }
        
        IEnumerator ProceedToNextStateWhenContextReady()
        {            
            while(!_contextManager.ContextReady)
                yield return null;
            CurrentRunthroughContext = _contextManager.CurrentRunthroughContext;
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
        
        void StartLoading()
        { 
            if(!_contextManager.CurrentlyGenerating)
                _contextManager.StartContextUpdate();
        }
    }
}