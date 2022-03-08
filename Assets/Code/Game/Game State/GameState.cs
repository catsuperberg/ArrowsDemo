using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.GameplayComponents.States;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using Game.Gameplay.Realtime.OperationSequence;
using Game.Gameplay.Realtime.PlayfildComponents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.GameState
{    
    public class GameState : MonoBehaviour, IStateChangeNotifier, IStateSignal
    {      
        IRuntimeFactory _runtimeFactory;  
        IUpdatedNotification _userContextNotifier;  
        
        public event EventHandler<StateEventArgs> OnStateChanged;
         
        FinishingScene _finishingScene;    
                
        AppState _previousState = AppState.Blank;
        AppState _lastFrameState = AppState.Blank;
        AppState _currentState = AppState.Blank;
        List<IStateReportableProcess> _processes = new List<IStateReportableProcess>();
        HashSet<SubState> _currentSubStates = new HashSet<SubState>(); 
        bool _stateDirty = false; 
        
        bool _stateChanged {get {return _currentState != _lastFrameState;}}         
        Task _CurrentStateTask;
        
        Playfield _level;
        Runthrough _runthrough;
        
        [Inject]
        public void Construct(IRuntimeFactory runtimeFactory,[Inject(Id = "userContextNotifier")] IUpdatedNotification userContextNotifier, ISequenceManager sequenceManager)
        {
             if(runtimeFactory == null)
                throw new ArgumentNullException("IRuntimeFactory isn't provided to GameState");
             if(userContextNotifier == null)
                throw new ArgumentNullException("IUpdatedNotification isn't provided to GameState");
             if(sequenceManager == null)
                throw new ArgumentNullException("ISequenceManager isn't provided to GameState");
                
            _runtimeFactory = runtimeFactory;
            _userContextNotifier = userContextNotifier;
            
            _processes.Add((IStateReportableProcess)sequenceManager);
            
            foreach(var process in _processes)
            {
                process.OnStateChanged += CollectSubProcessUpdate;                
            }
            CollectSubProcessUpdate(sequenceManager, new ProcessStateEventArgs(((IStateReportableProcess)sequenceManager).State));
            _CurrentStateTask = StateTick(_currentState);
            
            _userContextNotifier.OnUpdated += UserContextUpdated;
        }
        
        void Awake()
        {            
            _currentState = AppState.GameLaunch;
        }
                
        void Update()
        {
            if(!_CurrentStateTask.Status.Equals(TaskStatus.Running))
                _CurrentStateTask = StateTick(_currentState);
            if(_stateChanged)
            {
                _previousState = _lastFrameState;
                _lastFrameState = _currentState;
                _stateDirty = true;
            }
            
            if(_stateDirty)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => OnStateChanged?.Invoke(this, new StateEventArgs(_currentState, _currentSubStates)));
                _stateDirty = false;
            }
        }
        
        async Task StateTick(AppState newState)
        {           
            if(newState == AppState.DebugMenu || newState == AppState.Menu)
                Time.timeScale = 0;
            else if(Time.timeScale == 0)
                Time.timeScale = 1;
                
            switch (newState)
            {
                case AppState.GameLaunch:
                    await ProcessLaunch();
                    break;
                case AppState.StartScreen:
                    await ProcessStartScreen();
                    break;
                case AppState.GamePlay:
                    await ProcessGamePlay();
                    break;
                case AppState.FinishingCutscene:
                    await ProcessFinishingCutscene();
                    break;
                case AppState.PreAdTease:
                    await ProcessPreAdTease();
                    break;
            }
        }
        
        void CollectSubProcessUpdate(object sender, ProcessStateEventArgs e)
        {                
            var flagCollection = ProcessState.Blank;
            var newSubStates = new HashSet<SubState>(_currentSubStates);
            foreach(var process in _processes)
                flagCollection = flagCollection.SetFlag(process.State);
            if(flagCollection.HasFlag(ProcessState.Processing))
                newSubStates.Add(SubState.Generation);
            else
                newSubStates.Remove(SubState.Generation);
                
                
            if(!newSubStates.SetEquals(_currentSubStates))
            { 
                _currentSubStates = new HashSet<SubState>(newSubStates);
                _stateDirty = true;
            }     
        } 
        
        async Task ProcessLaunch()
        {
            if(_lastFrameState == AppState.Blank)
            {     
                await Task.Run(() => 
                    {
                        CreateLevel();
                        _currentState = AppState.StartScreen; 
                        _stateDirty = true;
                    });                          
            }
        }
        
        void CreateLevel()
        { 
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                _level = _runtimeFactory.GetLevel();
                _runthrough = _runtimeFactory.GetRunthrough(_level);
                }); 
        }
        
        
        async Task ProcessStartScreen()
        {
            if(_lastFrameState == AppState.PreAdTease)
            {     
                await Task.Run(() => 
                    {
                        CreateLevel();
                    });                          
            }
        }
        
        async Task ProcessGamePlay()
        {
            if(_lastFrameState == AppState.StartScreen)
            {
                await Task.Run(() => 
                {
                    _runthrough.OnFinished += OnGamePlayFinished;                    
                    UnityMainThreadDispatcher.Instance().Enqueue(
                        () => _runthrough.StartRun());
                });
            }
        }
        
        async Task ProcessFinishingCutscene()
        {
            if(_lastFrameState == AppState.GamePlay)
            {
                await Task.Run(() => 
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        _finishingScene = gameObject.AddComponent<FinishingScene>();
                        _finishingScene.OnFinished += OnFinishingSceneFinished;
                        _finishingScene.StartScene(_runthrough.ActiveProjectile.GetComponent<IDamageableWithTransforms>(), 
                            _level.Targets.GetComponent<IDamageableWithTransforms>());
                    });    
                });
            }
        }
        
        async Task ProcessPreAdTease()
        {
            if(_lastFrameState != AppState.PreAdTease)
            {                
                await Task.Delay(1500).ContinueWith(t => {_currentState = AppState.StartScreen;});
            }
        }
        
        void OnGamePlayFinished(object sender, EventArgs e)
        {
            _runthrough.OnFinished -= OnGamePlayFinished;
            _currentState = AppState.FinishingCutscene;
            Debug.Log("Gameplay Fnished");
        }     
        
        void OnFinishingSceneFinished(object sender, EventArgs e)
        {
            _finishingScene.OnFinished -= OnFinishingSceneFinished;
            _currentState = AppState.PreAdTease;
            
            GameObject.Destroy(_finishingScene);
            ClearLevel();
            Debug.Log("Finishing scene ended");
        }     
        
        void ClearLevel()
        {
            _runthrough.Destroy();
            _runthrough = null;            
            GameObject.Destroy(_level.GameObject);
            _level = null;
        }
        
        void UserContextUpdated(object caller, EventArgs e)
        {
            ClearLevel();
            CreateLevel();
        }
        
        
        public void SendStartGame()
        {
            if(_currentState == AppState.StartScreen)
                _currentState = AppState.GamePlay;    
        }
        
        public void SendUpgradeShop()
        {
            if(_currentState != AppState.UpgradeShop ||  _previousState == AppState.DebugMenu ||  _previousState == AppState.GameLaunch ||  _previousState == AppState.PreAdTease || _previousState == AppState.Ad)
                _currentState = AppState.UpgradeShop;
            else
                _currentState = _previousState;
        }
        
        public void SendPauseMenu()
        {
            if(_currentState != AppState.Menu ||  _previousState == AppState.DebugMenu ||  _previousState == AppState.GameLaunch ||  _previousState == AppState.PreAdTease || _previousState == AppState.Ad)
                _currentState = AppState.Menu;
            else
                _currentState = _previousState;
        }
        
        public void SendDebugMenu()
        {
            if(_currentState != AppState.DebugMenu ||  _previousState == AppState.GameLaunch ||  _previousState == AppState.PreAdTease || _previousState == AppState.Ad)
                _currentState = AppState.DebugMenu;  
        }
        
        public void SendPreviousState()
        {
            if(_previousState == AppState.GamePlay || _previousState == AppState.FinishingCutscene ||  _previousState == AppState.StartScreen)
                    _currentState = _previousState;
        }
    }
}