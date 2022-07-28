using AssetScripts.Instantiation;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using Timer = System.Timers.Timer;

namespace Game.Gameplay.Realtime
{
    public class RunthroughContextManager
    {
        IRunthroughFactory _runtimeFactory; 
        ProjectileInPlaceReplacer _projectileReplacer; 
        
        public RunthroughContext CurrentRunthroughContext {get; private set;} = null;
        RunthroughContext _contextToDestroy = null;
        
        public bool ContextReady {get {return CurrentContextValid && !RequestBeingProcessed;}}
        public bool RequestBeingProcessed {get => _generating || _requestWaiting;}
             
        bool _generating         
            {get => Convert.ToBoolean(Interlocked.Read(ref _lockingGenerationFlag));
             set => Interlocked.Exchange(ref _lockingGenerationFlag, Convert.ToInt64(value));}
        long _lockingGenerationFlag = 0;
        
        bool _requestWaiting => _multiRequestFilterTimer != null; 
        bool CurrentContextValid
            => CurrentRunthroughContext != null && CurrentRunthroughContext.Follower != null && CurrentRunthroughContext.Instatiator != null &&
                CurrentRunthroughContext.PlayfieldForRun != null && CurrentRunthroughContext.Projectile != null;
                
        Timer _multiRequestFilterTimer = null;
        bool _requestDuringGeneration = false;
        
        public RunthroughContextManager(IRunthroughFactory runtimeFactory, ProjectileInPlaceReplacer projectileReplacer)
        {
            _runtimeFactory = runtimeFactory ?? throw new ArgumentNullException(nameof(runtimeFactory));
            _projectileReplacer = projectileReplacer ?? throw new ArgumentNullException(nameof(projectileReplacer));
        }
        
        public void StartContextUpdate()
        {
            RegisterCurrentContextForDestruction();
            StartUpdate();
        }  
        
        public void RequestContextUpdate(int requestTimeoutMS = 250)
        {
            RegisterCurrentContextForDestruction();
            if(!_generating)
                UpdateIfRequestsStopForMs(requestTimeoutMS);
            else
                _requestDuringGeneration = true;
        }       
        
        public void UpdateProjectileToSelected()
        {
            var projectileParent = CurrentRunthroughContext.Projectile.transform.parent;
            var newProjectile = _projectileReplacer.CreateNewProjectileFromPrototype(CurrentRunthroughContext.Projectile);
            newProjectile.transform.SetParent(projectileParent);
            GameObject.Destroy(CurrentRunthroughContext.Projectile);
            CurrentRunthroughContext.Projectile = newProjectile;
        }      
        
        void UpdateIfRequestsStopForMs(int timeMs)
        {
            _multiRequestFilterTimer?.Dispose();
            _multiRequestFilterTimer = new Timer(timeMs);
            _multiRequestFilterTimer.Elapsed += UpdateOnContextChange;
            _multiRequestFilterTimer.Enabled = true;
        }
        
        void UpdateOnContextChange(object caller, EventArgs args)
        {
            _multiRequestFilterTimer?.Dispose();
            _multiRequestFilterTimer = null;
            StartUpdate();
        }
        
        
        void StartUpdate() => _ = UpdateContext();  
        
        void RegisterCurrentContextForDestruction()
        {
            if(CurrentRunthroughContext == null)
                return;
            _contextToDestroy = CurrentRunthroughContext; 
            CurrentRunthroughContext = null;
            // UnityMainThreadDispatcher.Instance().Enqueue(() 
            //     => {_contextToDestroy = CurrentRunthroughContext; CurrentRunthroughContext = null;});
        }  
        
        
        async Task UpdateContext()
        {  
            _generating = true;
            var _generatedContext = await _runtimeFactory.GetRunthroughContextHiden();
            await DestroyOldContext();
            await RevealContextPlayfield(_generatedContext);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {UpdateCurrentContext(_generatedContext);});
            UnityMainThreadDispatcher.Instance().Enqueue(() => {RenewRequestIfBoofered();});
            _generating = false;
        }
                
        async Task DestroyOldContext()
        {
            if(_contextToDestroy == null)
                return;
                
            var semaphore = new SemaphoreSlim(0,1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {ClearOldContext(); semaphore.Release();});
            await semaphore.WaitAsync();
        }
        
        void ClearOldContext()
        {
            if(_contextToDestroy == null)
                return;
                
            DestroyIfPresent(_contextToDestroy.Projectile);
            DestroyIfPresent(_contextToDestroy.FollowerObject);
            DestroyIfPresent(_contextToDestroy.PlayfieldObject);
            _contextToDestroy = null;
        }                
        
        void DestroyIfPresent(GameObject objectToDestroy)
        {
            if(objectToDestroy != null)
                GameObject.Destroy(objectToDestroy);
        }
        
        async Task RevealContextPlayfield(RunthroughContext newContext)
        {
            if(newContext.Instatiator.GetType() == typeof(InvisibleInstantiator))
                await newContext.Instatiator.UndoImplementationSpecifics();
        }
        
        void UpdateCurrentContext(RunthroughContext newContext)
        {
            CurrentRunthroughContext = newContext;
        }
        
        void RenewRequestIfBoofered()
        {
            if(!_requestDuringGeneration)
                return;
            
            _requestDuringGeneration = false;
            RequestContextUpdate();
        }
    }
}