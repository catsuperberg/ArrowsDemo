using Game.Gameplay.Realtime;
using Game.GameState.Context;
using System;
using UnityEngine;
using Zenject;

namespace Game.GameState
{    
    public class RunRestarter : MonoBehaviour, IRunRestarter
    {
        public GameObject GO {get {return this.gameObject;}}
        public PostRunContext Context {get; private set;}
        public event EventHandler OnProceedToRestart;   
        RunthroughContextManager _contextManager;
             
        [Inject]
        public void Initialize(RunthroughContextManager contextManager)
        {                
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));
        }    
                
        public void RequestRestart()
        {
            Context = new PostRunContext(playerSelectedReward: 0, showAdBeforeApplyingReward: false, 
                restartInsteadOfMenu: true);                
            _contextManager.RequestContextUpdate(20);            
            OnProceedToRestart?.Invoke(this, EventArgs.Empty);
        }
        
        public void RequestMenu()
        {
            Context = new PostRunContext(playerSelectedReward: 0, showAdBeforeApplyingReward: false, 
                restartInsteadOfMenu: false);                
            _contextManager.RequestContextUpdate(20);            
            OnProceedToRestart?.Invoke(this, EventArgs.Empty);
        }
    }
}