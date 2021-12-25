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
    public class GameState : MonoBehaviour, Controls.IDebugActions, IStateChangeNotifier
    {
        IMetaManager _metaManager;      
        IGamePlayManager _gamePlayManager;    
        ILevelManager _levelManager;  
        
        public event EventHandler<StateEventArgs> OnStateChanged;
         
        FinishingScene _finishingScene;    
                
        AppState _previousState = AppState.Blank;
        AppState _currentState = AppState.Blank;
        // List<SubState> _currentSubStates = new List<SubState>();
        List<IStateReportableProcess> _processes = new List<IStateReportableProcess>();
        HashSet<SubState> _currentSubStates = new HashSet<SubState>(); 
        bool _stateDirty = false; 
        
        bool _stateChanged {get {return _currentState != _previousState;}}         
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
            var gameplayControlls = new Controls();
            gameplayControlls.Debug.Enable();
            gameplayControlls.Debug.SetCallbacks(this);
            
            _currentState = AppState.GameLaunch;
        }
        
        void Update()
        {
            if(!_CurrentStateTask.Status.Equals(TaskStatus.Running))
                _CurrentStateTask = StateTick(_currentState);
            if(_stateChanged)
            {
                _previousState = _currentState;
                OnStateChanged?.Invoke(this, new StateEventArgs(_currentState, _currentSubStates));
            }
            
            if(_stateDirty)
            {
                OnStateChanged?.Invoke(this, new StateEventArgs(_currentState, _currentSubStates));
                _stateDirty = false;
            }
        }
        
        async Task StateTick(AppState newState)
        {           
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
                
            // Debug.Log("sender: " + sender);
            // Debug.Log("from CollectSubProcessUpdate()" + (int)e.State);
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
                // OnStateChanged?.Invoke(this, new StateEventArgs(_currentState, _currentSubStates));
            }     
        } 
        
        async Task ProcessLaunch()
        {
            if(_previousState == AppState.Blank)
            {     
                await Task.Run(() => 
                    {
                        LoadGame();
                        _currentState = AppState.StartScreen; 
                        OnStateChanged?.Invoke(this, new StateEventArgs(_currentState, _currentSubStates));
                    }).ConfigureAwait(false);                          
            }
        }
        
        void LoadGame()
        {
            BigInteger targetResult = _metaManager.GetNextTargetScore();
            Debug.LogWarning("GameState result is: " + targetResult);
            var context = _metaManager.GetContext();
            var spread = 15;
            OperationPairsSequence sequence =_metaManager.GenerateSequence(targetResult, spread); 
            Debug.LogWarning("Sequence generated: " + targetResult);
            UnityMainThreadDispatcher.Instance().Enqueue(() => _levelManager.InitializeLevel(context, sequence, targetResult)); 
            // _levelManager.InitializeLevel(context, sequence, targetResult);   
        }
        
        
        async Task ProcessStartScreen()
        {
            if(_previousState == AppState.PreAdTease)
            {     
                await Task.Run(() => 
                    {
                        LoadGame();
                    }).ConfigureAwait(false);                          
            }
        }
        
        async Task ProcessGamePlay()
        {
            if(_previousState == AppState.StartScreen)
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
            if(_previousState == AppState.GamePlay)
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
            Debug.Log("Finished");
        }     
        
        void OnFinishingSceneFinished(object sender, EventArgs e)
        {
            _finishingScene.OnFinished -= OnFinishingSceneFinished;
            _currentState = AppState.PreAdTease;
            Debug.Log("Finished");
        }     
        
        
        public void OnGenerateNewTarget(InputAction.CallbackContext context)
        {
            if(context.performed)
                return;
        }
        public void OnGenerateTrackForFirstTarget(InputAction.CallbackContext context)
        {     
            if(context.performed)
                return;
        }
        public void OnStartGame(InputAction.CallbackContext context)
        {
            if(context.performed)
                if(_currentState == AppState.StartScreen)
                    _currentState = AppState.GamePlay;
                else if (_currentState == AppState.PreAdTease)
                    _currentState = AppState.StartScreen;
                    
        }
    }
}