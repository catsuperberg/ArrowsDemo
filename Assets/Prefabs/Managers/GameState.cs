using GameMeta;
using GamePlay;
using Level;
using Sequence;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace State
{
    public class GameState : MonoBehaviour, Controls.IDebugActions
    {
        IMetaManager _metaManager;      
        IGamePlayManager _gamePlayManager;    
        ILevelManager _levelManager;  
        
         
        FinishingScene _finishingScene;    
                
        States _previousState = States.StartScreen;
        States _currentState = States.StartScreen;
        
        bool _stateChanged {get {return _currentState != _previousState;}}         
        
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
        }
        
        void Awake()
        {            
            var gameplayControlls = new Controls();
            gameplayControlls.Debug.Enable();
            gameplayControlls.Debug.SetCallbacks(this);
        }
        
        void Update()
        {
            if(_stateChanged)
            {
                ProcessState(_currentState);
                _previousState = _currentState;
            }
        }
        
        void ProcessState(States newState)
        {
            switch (newState)
            {
                case States.GamePlay:
                    ProcessGamePlay();
                    break;
                case States.FinishingCutscene:
                    ProcessFinishingCutscene();
                    break;
            }
        }
        
        void ProcessGamePlay()
        {
            if(_previousState == States.StartScreen)
            {
                var context = _metaManager.GetContext();
                var targetResult = _metaManager.GetNextTargetScore();
                var spread = 15;
                var sequence = _metaManager.GenerateSequence(targetResult, spread); 
                var level = _levelManager.InitializeLevel(context, sequence, targetResult);   
                
                _gamePlayManager.StartFromBeginning(level, context);
                _gamePlayManager.OnFinished += OnGamePlayFinished;
            }
        }
        
        void ProcessFinishingCutscene()
        {
            _finishingScene = gameObject.AddComponent<FinishingScene>();
            _finishingScene.OnFinished += OnFinishingSceneFinished;
            _finishingScene.StartScene(_gamePlayManager.ActiveProjectile.GetComponent<IDamageableWithTransforms>(), _levelManager.Targets.GetComponent<IDamageableWithTransforms>());
        }
        
        void OnGamePlayFinished(object sender, EventArgs e)
        {
            _gamePlayManager.OnFinished -= OnGamePlayFinished;
            _currentState = States.FinishingCutscene;
            Debug.Log("Finished");
        }     
        
        void OnFinishingSceneFinished(object sender, EventArgs e)
        {
            _finishingScene.OnFinished -= OnFinishingSceneFinished;
            _currentState = States.StartScreen;
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
                _currentState = States.GamePlay;
        }
    }
}