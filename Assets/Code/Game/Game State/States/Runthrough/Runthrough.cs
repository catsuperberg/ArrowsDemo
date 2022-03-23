using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using Game.Gameplay.Realtime;
using Game.Gameplay.Realtime.PlayfieldComponents;
using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameState
{    
    public class Runthrough : MonoBehaviour
    {
        Playfield _playfield;
        
        public event EventHandler OnFinished;
        Gameplay.Realtime.GameplayComponents.RunthroughState _state = Gameplay.Realtime.GameplayComponents.RunthroughState.Blank;
        List<Gameplay.Realtime.GameplayComponents.RunthroughState> _statesToGoThrough = new List<Gameplay.Realtime.GameplayComponents.RunthroughState>()
            { Gameplay.Realtime.GameplayComponents.RunthroughState.FlyingThroughTrack,
            Gameplay.Realtime.GameplayComponents.RunthroughState.FinishingScene,
            Gameplay.Realtime.GameplayComponents.RunthroughState.RunthroughOver};
        List<Gameplay.Realtime.GameplayComponents.RunthroughState>.Enumerator _stateEnumerator;  
            
        FlightThroughTrack _flyingState;
        FinishingScene _finishingSceneState;
        RewardCalculator _rewardCalculator;        
        
        public void Initialize(RunthroughContext runContext, RewardCalculator rewardCalculator)
        {
            if(runContext == null)
                throw new ArgumentNullException("RunthroughContext not provided to " + this.GetType().Name);
            if(rewardCalculator == null)
                throw new ArgumentNullException("RewardCalculator not provided to " + this.GetType().Name);
                
            _playfield = runContext.PlayfieldForRun;                 
            _rewardCalculator = rewardCalculator;      
            _stateEnumerator = _statesToGoThrough.GetEnumerator();     
            
            _flyingState = new FlightThroughTrack(runContext.Follower, runContext.Projectile);
        }
        
        void ProcessCurrentState()
        {
            switch(_state)
            {
                case Gameplay.Realtime.GameplayComponents.RunthroughState.FlyingThroughTrack:
                    StartFlight();
                    break;
                case Gameplay.Realtime.GameplayComponents.RunthroughState.FinishingScene:
                    StartFinishingScene();
                    break;
                case Gameplay.Realtime.GameplayComponents.RunthroughState.RunthroughOver:
                    RunFinished();
                    break;
            } 
        }
        
        void StartFlight()
        {
            _flyingState.OnFinished += CurrentStateFinished;
            _flyingState.StartRun();
        }
        
        void StartFinishingScene()
        {            
            _finishingSceneState = gameObject.AddComponent<FinishingScene>();
            _finishingSceneState.OnFinished += CurrentStateFinished;
            _finishingSceneState.StartScene(_flyingState.ActiveProjectile.GetComponent<IDamageableWithTransforms>(), 
                _playfield.Targets.GetComponent<IDamageableWithTransforms>(), _rewardCalculator);
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
        
        void RunFinished()
        {                        
            OnFinished?.Invoke(this, EventArgs.Empty);
        }     
        
        public void StartRun()
        {          
            AdvanceState();
        }
        
        public void Destroy()
        {       
            _flyingState.Destroy();
            _flyingState = null;
            _playfield = null;
            GameObject.Destroy(_finishingSceneState);
            foreach (Transform child in gameObject.transform) {
                GameObject.Destroy(child.gameObject);
            }
            GameObject.Destroy(gameObject);
        }
    }
}