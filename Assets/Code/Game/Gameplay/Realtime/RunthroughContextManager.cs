using AssetScripts.Instantiation;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime
{
    public class RunthroughContextManager
    {
        IRunthroughFactory _runtimeFactory;  
        
        public RunthroughContext CurrentRunthroughContext {get; private set;} = null;
        public bool ContextReady {get {return (CurrentContextValid() && !CurrentlyGenerating);}}
        public bool CurrentlyGenerating {get; private set;} = false;
        
        bool CurrentContextValid()
        {
            return CurrentRunthroughContext != null && CurrentRunthroughContext.Follower != null && CurrentRunthroughContext.Instatiator != null &&
                CurrentRunthroughContext.PlayfieldForRun != null && CurrentRunthroughContext.Projectile != null;
        }
        
        private RunthroughContext _nextRunthroughContext = null;
        
        public RunthroughContextManager(IRunthroughFactory runtimeFactory)
        {            
            if(runtimeFactory == null)
                throw new ArgumentNullException("IRuntimeFactory isn't provided to " + this.GetType().Name);
                
            _runtimeFactory = runtimeFactory;
        }
        
        public void StartContextUpdate()
        {
            _ = UpdateContext();
        }
        
        public async Task<RunthroughContext> GetContextWhenReady()
        {
            if(ContextReady)
                return CurrentRunthroughContext;
            
            if(!CurrentlyGenerating)
                await UpdateContext();
                        
            return CurrentRunthroughContext;
        }
        
        async Task UpdateContext()
        {  
            CurrentlyGenerating = true;
            await CreateLevel(); 
            HideCurrentLevel();
            ShowNextLevel();
            ClearLevel(); 
            CurrentRunthroughContext = _nextRunthroughContext;
            _nextRunthroughContext = null;
            CurrentlyGenerating = false;
        }
        
        async Task CreateLevel()
        {          
            _nextRunthroughContext = await _runtimeFactory.GetRunthroughContextHiden();          
        }
        
        void HideCurrentLevel()
        {
            if(!CurrentContextValid())
                return;
                
            if(CurrentRunthroughContext.Instatiator.GetType() == typeof(InvisibleInstantiator))
                CurrentRunthroughContext.Instatiator.RedoImplementationSpecifics();
        }
        
        void ShowNextLevel()
        {
            if(_nextRunthroughContext.Instatiator.GetType() == typeof(InvisibleInstantiator))
                _nextRunthroughContext.Instatiator.UndoImplementationSpecifics();
        }
        
        void ClearLevel()
        {      
            if(!CurrentContextValid())
                return;
            var tempContext = CurrentRunthroughContext;
            UnityMainThreadDispatcher.Instance().Enqueue(() => {ClearingCurrentLevel(tempContext); tempContext = null;});
        }
        
        void ClearingCurrentLevel(RunthroughContext context)
        {              
            GameObject.Destroy(context.Projectile);
            GameObject.Destroy(context.Follower.Transform.gameObject);
            GameObject.Destroy(context.PlayfieldForRun.GameObject);
            context = null;
        }
    }
}