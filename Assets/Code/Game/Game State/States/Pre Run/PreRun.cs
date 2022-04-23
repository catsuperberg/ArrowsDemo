using Game.Gameplay.Realtime;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
// using System.Timers;
using UnityEngine;

using Timer = System.Timers.Timer;

namespace Game.GameState
{    
    public class PreRun : MonoBehaviour
    {
        [SerializeField]
        PreRunUI _UI;
        
        IRuntimeFactory _runtimeFactory;  
        IUpdatedNotification _userContextNotifier; 
        
        public RunthroughContext CurrentRunthroughContext {get; private set;} = null;
        private RunthroughContext _nextRunthroughContext = null;
        private Timer _contextUpdateTimer = new Timer();
        private bool _currentlyGenerating = false;
        
        public event EventHandler OnProceedToNextState;
                
        public void Initialize(IRuntimeFactory runtimeFactory, IUpdatedNotification userContextNotifier)
        {
            if(runtimeFactory == null)
                throw new ArgumentNullException("IRuntimeFactory isn't provided to " + this.GetType().Name);
             if(userContextNotifier == null)
                throw new ArgumentNullException("IUpdatedNotification isn't provided to " + this.GetType().Name);
                
            _runtimeFactory = runtimeFactory;
            _userContextNotifier = userContextNotifier;
            
            _UI.OnStartRunthrough += UIStartButtonPressed;
            _userContextNotifier.OnUpdated += UserContextUpdated;
            
            _ = StartLoading();
        }
        
        void UIStartButtonPressed(object caller, EventArgs args)
        {                
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
                
        void UserContextUpdated(object caller, EventArgs e)
        {            
            if(CurrentRunthroughContext != null && !_currentlyGenerating) // HACK Reward aplier updates values in the end of a frame and aparently update event buffers until next, so on creation of new PreRun both StartLoading and update call updating level
                UpdateLevelIfUpdatesStopForMs(200);
        }
        
        void UpdateLevelIfUpdatesStopForMs(int timeMs)
        {
            _contextUpdateTimer.Dispose();
            _contextUpdateTimer = new Timer(timeMs);
            _contextUpdateTimer.Elapsed += UpdateOnContextChange;
            _contextUpdateTimer.Enabled = true;
        }
        
        void UpdateOnContextChange(object caller, EventArgs args)
        {
            _contextUpdateTimer.Dispose();
            UnityMainThreadDispatcher.Instance().Enqueue(() => {_ = UpdateLevel();}); 
            // Task.Run(UpdateLevel);
            // _ = StartUpdatingLevel();
        }
        
        async Task StartLoading()
        { 
            StartCoroutine(ShowLoadingScreenUntilPlayfieldPresent());
                
            await UpdateLevel();
        }
        
        // async Task StartUpdatingLevel()
        // { 
        //     await UpdateLevel();
        // }
        
        IEnumerator ShowLoadingScreenUntilPlayfieldPresent()
        {            
            _UI.SwithchToLoadingScreen(); 
            while(CurrentRunthroughContext == null)
                yield return null;
            _UI.SwithchToStartScreen(); 
        }
        
        async Task UpdateLevel()
        {  
            _currentlyGenerating = true;
            await CreateLevel(); 
            await ClearLevel(); 
            CurrentRunthroughContext = _nextRunthroughContext;
            _nextRunthroughContext = null;
            _currentlyGenerating = false;
        }
        
        async Task CreateLevel()
        {          
            _nextRunthroughContext = await _runtimeFactory.GetRunthroughContext();          
        }
        
        async Task ClearLevel()
        {      
            var clearingLevelSemaphore = new SemaphoreSlim(0, 1);
            if(CurrentRunthroughContext == null)
                return;
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(ClearingCurrentLevelCoroutine(clearingLevelSemaphore));});
            await clearingLevelSemaphore.WaitAsync();    
            
        }
        
        IEnumerator ClearingCurrentLevelCoroutine(SemaphoreSlim semaphore)
        {              
            Destroy(CurrentRunthroughContext.Projectile);
            Destroy(CurrentRunthroughContext.Follower.Transform.gameObject);
            Destroy(CurrentRunthroughContext.PlayfieldForRun.GameObject);
            CurrentRunthroughContext = null;
            semaphore.Release();
            yield return null;
        }
        
        void OnDestroy()
        {
            _userContextNotifier.OnUpdated -= UserContextUpdated;
            _userContextNotifier = null;
            Destroy(_UI);
            _UI = null;
            _runtimeFactory = null;
            _userContextNotifier = null;            
        }
    }
}