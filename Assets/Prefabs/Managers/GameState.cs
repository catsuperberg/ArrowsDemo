using GameMeta;
using GamePlay;
using Level;
using Sequence;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace State
{
    public class GameState : MonoBehaviour, Controls.IDebugActions
    {
        IMetaGame _meta;      
        IGamePlayManager _gamePlayManager;    
        ILevelManager _levelManager;  
                
        States _previousState = States.StartScreen;
        States _currentState = States.StartScreen;
        
        bool _stateChanged {get {return _currentState != _previousState;}} 
        
        
        [Inject]
        public void Construct(IMetaGame meta, IGamePlayManager gamePlayManager, ILevelManager levelManager)
        {
             if(meta == null)
                throw new System.Exception("IMetaGame isn't provided to GameState");
             if(gamePlayManager == null)
                throw new System.Exception("IGamePlayManager isn't provided to GameState");
             if(levelManager == null)
                throw new System.Exception("ILevelManager isn't provided to GameState");
                
            _meta = meta;
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
            }
        }
        
        void ProcessGamePlay()
        {
            if(_previousState == States.StartScreen)
            {
                // FIXME IMetaGameManager responsibility
                var startTime = Time.realtimeSinceStartup;
                
                var context = new SequenceContext(900, 10, 30);
                var targetResult = _meta.GetAverageSequenceResult(context, 2000);
                var spread = 15;
                var sequence = _meta.GenerateSequence(targetResult, spread, context); 
                var level = _levelManager.InitializeLevel(context, sequence, targetResult);
                
                var generationTime = Time.realtimeSinceStartup - startTime;
                Debug.Log("Time to generate track: " + generationTime);
                
                
                _gamePlayManager.StartFromBeginning(level, context);
            }
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