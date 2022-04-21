using Game.Gameplay.Realtime;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.GameState
{    
    // TODO full rework of states needed, plus async playfield creation
    // HACK full rework of states needed, plus async playfield creation
    public class PreRun : MonoBehaviour
    {
        [SerializeField]
        PreRunUI _UI;
        
        IRuntimeFactory _runtimeFactory;  
        IUpdatedNotification _userContextNotifier; 
        
        PreRunStates _previousState = PreRunStates.Blank;
        PreRunStates _state = PreRunStates.Blank;
        public RunthroughContext NextRunthroughContext {get; private set;} = null;
        
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
            
            Task.Run(() => {CreateLevel();});
            
            // UnityMainThreadDispatcher.Instance().Enqueue(() => { 
            //     StartLoading();}); 
        }
        
        void UIStartButtonPressed(object caller, EventArgs args)
        {
            if(_state != PreRunStates.Ready)
                throw new Exception("Pre run state isn't ready, but start button is pressed");
                
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
                
        void UserContextUpdated(object caller, EventArgs e)
        {            
            UpdateLevel();
        }
        
        void StartLoading()
        {
            _state = PreRunStates.Launching;
            _UI.SwithchToLoadingScreen();  
            UpdateLevel();
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => { 
                _UI.SwithchToStartScreen();});  
            _state = PreRunStates.Ready;
        }
        
        // void Update()
        // {
        //     checkIfLevelReady();
        //     if(_previousState != _state)
        //         if(_previousState == PreRunStates.Launching && _state == PreRunStates.Ready)
        //             _UI.SwithchToStartScreen();
        //         // ProcessState();
        // }
        
        // void checkIfLevelReady()
        // {
        //     if(NextRunthroughContext != null)
        //         _state = PreRunStates.Ready;
        //     else
        //         Debug.LogWarning("NextRunthroughContext = null");
        // }
        
        void OnDestroy()
        {
            _userContextNotifier.OnUpdated -= UserContextUpdated;
            _userContextNotifier = null;
            Destroy(_UI);
            _UI = null;
            _runtimeFactory = null;
            _userContextNotifier = null;            
        }
        
        // void ProcessState()
        // {
        //     switch (_state)
        //     {
        //         // case PreRunStates.Launching:
        //         //     UpdateLevel();
        //         //     break;        
        //         // case PreRunStates.WaitingForNewPlayfield:                     
        //         //     break;                          
        //         case PreRunStates.Ready:
        //             if(_previousState == PreRunStates.Launching)
        //                 _UI.SwithchToStartScreen();
        //             break;
        //         default:
        //             break;
        //     }
        //     _previousState = _state;
        // }
        
        void UpdateLevel()
        {  
            UnityMainThreadDispatcher.Instance().Enqueue(() => { 
                    ClearLevel();                    
                    CreateLevel();});   
        }
        
        async void CreateLevel()
        {          
            NextRunthroughContext = await _runtimeFactory.GetRunthroughContext();          
        }
        
        void ClearLevel()
        {      
            if(NextRunthroughContext == null)
                return;                    
                                
            GameObject.Destroy(NextRunthroughContext.Projectile);
            GameObject.Destroy(NextRunthroughContext.Follower.Transform.gameObject);
            GameObject.Destroy(NextRunthroughContext.PlayfieldForRun.GameObject);
            
            
            NextRunthroughContext = null;
        }
    }
}