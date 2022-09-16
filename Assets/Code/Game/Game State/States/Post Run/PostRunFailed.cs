using Game.Gameplay.Realtime;
using Game.GameState.Context;
using System;
using UnityEngine;

namespace Game.GameState
{    
    public class PostRunFailed : MonoBehaviour, IPostRun
    {
        public GameObject GO {get {return this.gameObject;}}
        public PostRunContext Context {get => _restarter.Context;}
        public event EventHandler OnProceedToNextState;   
        
        RunRestarter _restarter;
             
        public void Initialize(RunthroughContextManager contextManager)
        {
            _restarter = gameObject.AddComponent<RunRestarter>();
            _restarter.Initialize(contextManager);
            _restarter.OnProceedToRestart += Request;
        }    
                
        public void RequestRestart()
            => _restarter.RequestRestart();
        
        public void RequestMenu()
            => _restarter.RequestMenu();
            
        void Request(object caller, EventArgs args)
            => OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        
        void OnDestroy()
        {
            _restarter.OnProceedToRestart -= Request;            
        }
    }
}