using GameMeta;
using GamePlay;
using Level;
using Sequence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace State
{    
    public class GameState : MonoBehaviour, IStateChangeNotifier, IStateSignal//, Controls.IDebugActions, Controls.IUIActions
    {
        IMetaManager _metaManager;      
        IGamePlayManager _gamePlayManager;    
        ILevelManager _levelManager;  
        
        public event EventHandler<StateEventArgs> OnStateChanged;
         
        FinishingScene _finishingScene;    
                
        AppState _previousState = AppState.Blank;
        AppState _lastFrameState = AppState.Blank;
        AppState _currentState = AppState.Blank;
        // List<SubState> _currentSubStates = new List<SubState>();
        List<IStateReportableProcess> _processes = new List<IStateReportableProcess>();
        HashSet<SubState> _currentSubStates = new HashSet<SubState>(); 
        bool _stateDirty = false; 
        
        bool _stateChanged {get {return _currentState != _lastFrameState;}}         
        Task _CurrentStateTask;
        
        [Inject]
        public void Construct(IMetaManager meta, IGamePlayManager gamePlayManager, ILevelManager levelManager)
        {
             if(meta == null)
                throw new System.Exception("IMetaManager isn't provided to GameState");
             if(gamePlayManager == null)
                throw new System.Exception("IGamePlayManager isn't provided to GameState");
             if(levelManager == null)
                throw new System.Exception("ILevelManager isn't provided to GameState");
                
            _metaManager = meta;
            _gamePlayManager = gamePlayManager;
            _levelManager = levelManager;
            
            
            _processes.Add(_metaManager);
            
            foreach(var process in _processes)
            {
                process.OnStateChanged += CollectSubProcessUpdate;                
            }
            CollectSubProcessUpdate(_metaManager, new ProcessStateEventArgs(_metaManager.State));
            _CurrentStateTask = StateTick(_currentState);
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
            var targetResult = _metaManager.GetNextTargetScore();
            var context = _metaManager.GetContext();
            var spread = 15;
            var sequence =_metaManager.GenerateSequence(targetResult, spread);   
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _levelManager.InitializeLevel(context, sequence, targetResult);                
                stopwatch.Stop();
                Debug.LogWarning("InitializeLevel() took: " + stopwatch.ElapsedMilliseconds + " ms");   
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
                    _gamePlayManager.OnFinished += OnGamePlayFinished;                    
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _gamePlayManager.StartFromBeginning(_levelManager.Level, _metaManager.GetContext()));
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
                        _finishingScene.StartScene(_gamePlayManager.ActiveProjectile.GetComponent<IDamageableWithTransforms>(), _levelManager.Targets.GetComponent<IDamageableWithTransforms>());
                    });    
                });
            }
        }
        
        void OnGamePlayFinished(object sender, EventArgs e)
        {
            _gamePlayManager.OnFinished -= OnGamePlayFinished;
            _currentState = AppState.FinishingCutscene;
            Debug.Log("Gameplay Fnished");
        }     
        
        void OnFinishingSceneFinished(object sender, EventArgs e)
        {
            _finishingScene.OnFinished -= OnFinishingSceneFinished;
            _currentState = AppState.PreAdTease;
            Debug.Log("Finishing scene ended");
        }     
        
        
        public void SendStartGame()
        {
            if(_currentState == AppState.StartScreen)
                _currentState = AppState.GamePlay;                
            else if (_currentState == AppState.PreAdTease) // TEMP until tease and ad working
                _currentState = AppState.StartScreen;
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