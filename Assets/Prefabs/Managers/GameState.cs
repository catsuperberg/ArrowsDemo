using GameMeta;
using GamePlay;
using Level;
using Sequence;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using PeterO.Numbers;

namespace State
{
    public class GameState : MonoBehaviour, Controls.IDebugActions
    {
        IMetaManager _metaManager;      
        IGamePlayManager _gamePlayManager;    
        ILevelManager _levelManager;  
                
        States _previousState = States.StartScreen;
        States _currentState = States.StartScreen;
        
        bool _stateChanged {get {return _currentState != _previousState;}} 
        
        // HACK feels wrong to pass it through here
        GameObject _targets = null;
        
        
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
            if(_currentState == States.FinishingCutscene)
                FinishingCutsceneUpdate();
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
                _targets = _levelManager.Targets;            
                
                _gamePlayManager.StartFromBeginning(level, context);
                _gamePlayManager.OnFinished += OnGamePlayFinished;
            }
        }
        
        void ProcessFinishingCutscene()
        {
            var projectile = _gamePlayManager.ActiveProjectile.GetComponent<IProjectileObject>();
            var sceneTime = 4;
            var target = _targets.GetComponent<IDamageable>();
            var targetPoints = EInteger.FromString(target.DamagePoints.ToString());
            __count = targetPoints;
            __finalCount = 500;        
            var finalCountPart = EDecimal.FromEInteger(__finalCount).Divide(__count, EContext.Binary64);
            var k = finalCountPart.Log(EContext.Binary64).Divide(sceneTime, EContext.Binary64);
            __halfLife = EDecimal.FromDouble(Math.Log(0.5)).Divide(k, EContext.Binary64);
            __startTime = Time.realtimeSinceStartup;
        }
        
        EInteger __count;
        EInteger __finalCount;
        EDecimal __halfLife;
        float __startTime;        
        
        void FinishingCutsceneUpdate()
        {
            var target = _targets.GetComponent<IDamageable>();
            var targetPoints =EDecimal.FromString(target.DamagePoints.ToString());
            if(targetPoints.CompareTo(__finalCount) >= 0)
            {
                var frameCoeff = ((EDecimal.FromDouble(Time.deltaTime).Divide(__halfLife, EContext.Binary64)) * 
                    (EDecimal.FromDouble(Math.Log(0.5)))).Exp(EContext.Binary64);
                var undecayed = (__count * frameCoeff).ToEInteger();     
                var frameDamage = __count - undecayed;
                __count = undecayed;           
                var intDamage = BigInteger.Parse(frameDamage.ToString());
                target.Damage(intDamage);                
            }
            else
            {
                Debug.Log("Cutscene took: " + (Time.realtimeSinceStartup - __startTime));
                _currentState = States.Ad;
            }
            
        }
        
        void OnGamePlayFinished(object sender, EventArgs e)
        {
            _gamePlayManager.OnFinished -= OnGamePlayFinished;
            _currentState = States.FinishingCutscene;
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