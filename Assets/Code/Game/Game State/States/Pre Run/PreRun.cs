using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.PlayfieldComponents;
using System;
using UnityEngine;

namespace Game.GameState
{    
    public class PreRun : MonoBehaviour
    {
        [SerializeField]
        PreRunUI _UI;
        
        IRuntimeFactory _runtimeFactory;  
        IUpdatedNotification _userContextNotifier; 
        
        PreRunStates _previousState = PreRunStates.Blank;
        PreRunStates _state = PreRunStates.Blank;
        public RunthroughContext NextRunthroughContext {get; private set;}
        
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
            userContextNotifier.OnUpdated += UserContextUpdated;
            
            StartLoading();
        }
        
        void UIStartButtonPressed(object caller, EventArgs args)
        {
            if(_state != PreRunStates.Ready)
                throw new Exception("Pre run state isn't ready, but start button is pressed");
                
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
                
        void UserContextUpdated(object caller, EventArgs e)
        {
            ClearLevel();
            CreateLevel();
        }
        void StartLoading()
        {
            _state = PreRunStates.Launching;
            _UI.SwithchToLoadingScreen();            
        }
        
        void Update()
        {
            if(_previousState != _state)
                ProcessState();
        }
        
        void ProcessState()
        {
            switch (_state)
            {
                case PreRunStates.Launching:
                    CreateLevel();
                    break;        
                case PreRunStates.WaitingForNewPlayfield:
                    
                    break;                          
                case PreRunStates.Ready:
                    if(_previousState == PreRunStates.Launching)
                        _UI.SwithchToStartScreen();
                    break;
                default:
                    break;
            }
            _previousState = _state;
        }
        
        void CreateLevel()
        {            
            if(_state != PreRunStates.Launching) 
                _state = PreRunStates.WaitingForNewPlayfield;
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                NextRunthroughContext = _runtimeFactory.GetRunthroughContext();
                _state = PreRunStates.Ready;
                }); 
        }
        
        void ClearLevel()
        {      
            if(NextRunthroughContext == null)
                return;
            
            GameObject.Destroy(NextRunthroughContext.PlayfieldForRun.GameObject);
            GameObject.Destroy(NextRunthroughContext.Follower.Transform.gameObject);
            GameObject.Destroy(NextRunthroughContext.Projectile);
            
            NextRunthroughContext = null;
        }
    }
}