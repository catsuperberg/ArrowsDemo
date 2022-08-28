using Game.Gameplay.Realtime;
using Game.GameState.Context;
using System;
using UnityEngine;

namespace Game.GameState
{    
    public class PostRunFailedRun : MonoBehaviour, IPostRun
    {
        public GameObject GO {get {return this.gameObject;}}
        public PostRunContext Context {get; private set;}
        public event EventHandler OnProceedToNextState;   
             
        public void Initialize(RunthroughContextManager contextManager)
        {
             if(contextManager == null)
                throw new ArgumentNullException("RunthroughContextManager isn't provided to " + this.GetType().Name);
                
            contextManager.StartContextUpdate();
        }    
                
        public void RequestRestart()
        {
            Context = new PostRunContext(playerSelectedReward: 0, showAdBeforeApplyingReward: false, 
                restartInsteadOfMenu: true);
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
        
        public void RequestMenu()
        {
            Context = new PostRunContext(playerSelectedReward: 0, showAdBeforeApplyingReward: false, 
                restartInsteadOfMenu: false);
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
    }
}