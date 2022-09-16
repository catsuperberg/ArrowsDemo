using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using Game.Gameplay.Realtime.PlayfieldComponents;
using Game.GameState.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Game.GameState
{    
    public class Runthrough : MonoBehaviour, IRunRestarter
    {        
        [SerializeField]
        private RunthroughUI _UI;
        
        public RunFinishContext FinishingContext {get; private set;}
        
        Playfield _playfield;
        
        RunthroughState _state = RunthroughState.Blank;
        List<RunthroughState> _statesToGoThrough = new List<RunthroughState>()
            { RunthroughState.FlyingThroughTrack,
            RunthroughState.FinishingScene,
            RunthroughState.RunthroughOver};
        List<RunthroughState>.Enumerator _stateEnumerator;  
        List<RunthroughState> PausableStates = new List<RunthroughState>{RunthroughState.FlyingThroughTrack};
        bool _paused = false;        
            
        FlightThroughTrack _flyingState;
        FinishingScene _finishingSceneState;
        RewardCalculator _rewardCalculator;    
        IProjectile _projectile;    
        
        public event EventHandler OnProceedToNextState;  
        
        IRunRestarter _restarter;
        public PostRunContext Context {get => _restarter.Context;} 
        public event EventHandler OnProceedToRestart;
        
        void OnDestroy()
        {            
            DestroyRun();
            _restarter.OnProceedToRestart -= RequestRestart;
        }
        
        public void Initialize(RunthroughContext runContext, RewardCalculator rewardCalculator)
        {
            if(runContext == null)
                throw new ArgumentNullException("RunthroughContext not provided to " + this.GetType().Name);
                
            _playfield = runContext.PlayfieldForRun;   
            _rewardCalculator = rewardCalculator ?? throw new ArgumentNullException(nameof(rewardCalculator));      
            _stateEnumerator = _statesToGoThrough.GetEnumerator();   
            
            _projectile = runContext.Projectile.GetComponent<IProjectile>();
            _projectile.OnUpdated += CheckForPlayerFail;
            AttachUIRequests();
            
            _flyingState = new FlightThroughTrack(runContext.Follower, runContext.Projectile); 
            
            _restarter = GetComponentsInChildren<IRunRestarter>()
                .First(restarter => restarter.GetHashCode() != this.GetHashCode());
            _restarter.OnProceedToRestart += RequestRestart;
        }
        
        void RequestRestart(object caller, EventArgs args)
            => OnProceedToRestart?.Invoke(this, EventArgs.Empty);
        
        void CheckForPlayerFail(object caller, EventArgs args)
        {
            if(_projectile.Count <= 0 && _state == RunthroughState.FlyingThroughTrack)
                ProceedToFail();
        }
        
        void ProceedToFail()
        {            
            FinishingContext = new RunFinishContext(0, runFailed: true);
            DestroyAndProceedToNextState();
        }
        
        void AttachUIRequests()
        {
            _UI.OnPauseUnpauseRequest += DecideOnPause;
            _UI.OnPauseRequest += EnablePause;
            _UI.OnContinueGameplayRequest += DisablePause;
        }
                
        void DecideOnPause(object sender, EventArgs e)
        {
            if(!PausableStates.Contains(_state))
                return;
                
            if(_paused)
                DisablePause(sender, EventArgs.Empty);
            else
                EnablePause(sender, EventArgs.Empty);
        }
        
        void EnablePause(object sender, EventArgs e)
        {
            if(!PausableStates.Contains(_state))
                return;
                
            _paused = true;
            _UI.SwithchToPause();
            ProcessPause();
        }
        
        void DisablePause(object sender, EventArgs e)
        {
            if(!PausableStates.Contains(_state))
                return;
                
            _paused = false;
            _UI.SwithchToGameplay();
            ProcessPause();
        }        
        
        void ProcessPause()
        {
            _flyingState.SetPaused(_paused);
        }
        
        void ProcessCurrentState()
        {
            switch(_state)
            {
                case RunthroughState.FlyingThroughTrack:
                    StartFlight();
                    break;
                case RunthroughState.FinishingScene:
                    StartFinishingScene();
                    break;
                case RunthroughState.RunthroughOver:
                    RunFinished();
                    break;
            } 
        }
        
        void StartFlight()
        {
            _flyingState.OnFinished += CurrentStateFinished;
            _flyingState.StartRun();
            _UI.SwithchToGameplay();
        }        
                
        public void StartRun()
        {          
            AdvanceState();
        }
        
        
        void CurrentStateFinished(object sender, EventArgs e)
        {
            AdvanceState();
        }
        
        void AdvanceState()
        {            
            _stateEnumerator.MoveNext();
            _state = _stateEnumerator.Current; 
            ProcessCurrentState();
        }
        
        void StartFinishingScene()
        {            
            _UI.SwithchToFinishingScene();
            var rewardDisplay = _UI.GetComponentInChildren<RewardDisplay>();
            rewardDisplay.Initialize(_rewardCalculator);
            _finishingSceneState = gameObject.AddComponent<FinishingScene>();
            _finishingSceneState.OnFinished += CurrentStateFinished;
            _finishingSceneState.StartScene(_flyingState.ActiveProjectile.GameObject.GetComponent<IDamageableWithTransforms>(), 
                _playfield.Targets.GetComponent<IDamageableWithTransforms>(), _rewardCalculator);
        }
        
        void RunFinished()
        {               
            FinishingContext = new RunFinishContext(_rewardCalculator.Reward, runFailed: false);
            DestroyAndProceedToNextState();
        }     
        
        void DestroyAndProceedToNextState()
        {            
            DestroyRun();
            OnProceedToNextState?.Invoke(this, EventArgs.Empty);
        }
        
        public void DestroyRun()
        {   
            _stateEnumerator.Dispose();
            _state = RunthroughState.Blank; 
                
            if(_playfield != null)
            {
                Destroy(_playfield.GameObject);
                _playfield = null;
                
            }
            if(_flyingState != null)
            {
                _flyingState.DestroyFlight();
                _flyingState = null;
            }
            if(_finishingSceneState != null)
                Destroy(_finishingSceneState);
            foreach (Transform child in gameObject.transform) {
                Destroy(child.gameObject);
            }
        }
    }
}