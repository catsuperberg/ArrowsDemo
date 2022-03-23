using Game.Gameplay.Realtime.PlayfieldComponents.Track;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.GameplayComponents.GameCamera;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.PlayfieldComponents;
using Input.ControllerComponents;
using System;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents
{
    public class FlightThroughTrack
    {
        ITrackFollower _follower;    
        
        public event EventHandler OnFinished;
        public GameObject ActiveProjectile {get; private set;} = null;
        
        TouchTranslationMovementController _movementController;
        
        public FlightThroughTrack(ITrackFollower follower, GameObject activeProjectile)
        {
            if(follower == null)
                throw new ArgumentNullException("ITrackFollower not provided to RunScene");
                
            _follower = follower;
            
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = _follower.Transform;
                            
            ActiveProjectile = activeProjectile;
            
            
            _follower.OnFinished += FlightFinished;
        }
        
        public void Destroy()
        {       
            _follower.OnFinished -= FlightFinished;
            _follower = null;
            if(OnFinished != null)
            foreach (var d in OnFinished.GetInvocationList())
                OnFinished -= (EventHandler)d;
            OnFinished = null;  
            if(_movementController != null)         
            { 
                GameObject.Destroy(_movementController);  
                _movementController = null;
            }
            ActiveProjectile = null;
        }
        
        void FlightFinished(object sender, EventArgs e)
        {            
            GameObject.Destroy(_movementController);  
            _movementController = null;
            OnFinished?.Invoke(this, EventArgs.Empty);
        }     
        
        public void StartRun()
        {          
            _movementController = ActiveProjectile.AddComponent<TouchTranslationMovementController>();
            _movementController.Init();
            _follower.StartMovement();
        }
    }    
}