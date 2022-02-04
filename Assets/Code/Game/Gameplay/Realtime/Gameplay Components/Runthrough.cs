using Game.Gameplay.Realtime.PlayfildComponents.Track;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.GameplayComponents.GameCamera;
using Game.Gameplay.Realtime.GameplayComponents.Projectiles;
using Game.Gameplay.Realtime.PlayfildComponents;
using Input.ControllerComponents;
using System;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents
{
    public class Runthrough
    {
        ITrackFollower _follower;    
        IProjectileProvider _projectileGenerator;  
        
        public event EventHandler OnFinished;
        public GameObject ActiveProjectile {get; private set;} = null;
        
        ButtonsMovementController _movementController;
        TouchTranslationMovementController _movementController2;
        
        public Runthrough(ITrackFollower follower, IProjectileProvider projectileGenerator, Playfield level, SequenceContext context)
        {
            if(follower == null)
                throw new System.Exception("ITrackFollower not provided to RunScene");
            if(projectileGenerator == null)
                throw new System.Exception("IProjectileProvider not provided to RunScene");
                
            _follower = follower;
            _projectileGenerator = projectileGenerator;
            
            _follower.SetSplineToFollow(level.TrackSpline, 0);
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = _follower.Transform;
                            
            ActiveProjectile = _projectileGenerator.CreateArrows(context.InitialValue, 12f); // HACK arbitrary width used for movement width
            ActiveProjectile.transform.SetParent(_follower.Transform);
            
            var _movementController = ActiveProjectile.AddComponent<ButtonsMovementController>(); // HACK i think what class is used for contrller should be defined in construction root
            _movementController.Init();
            var _movementController2 = ActiveProjectile.AddComponent<TouchTranslationMovementController>();
            _movementController2.Init();
            
            _follower.OnFinished += GamePlayFinished;
        }
        
        void GamePlayFinished(object sender, EventArgs e)
        {
            _follower.OnFinished -= GamePlayFinished;
            _follower = null;
            _projectileGenerator = null;
            GameObject.Destroy(_movementController);  
            GameObject.Destroy(_movementController2);
            _movementController = null;
            _movementController2 = null;
                
            
            var newCameraTarget = new GameObject("CameraTarget");
            var arrowsTransform = ActiveProjectile.GetComponentInChildren<TMPro.TMP_Text>().gameObject.transform;
            newCameraTarget.transform.position = arrowsTransform.position + new UnityEngine.Vector3(0, 14, 6);
            var additionalRotation =  UnityEngine.Quaternion.Euler(25, 0, 0);
            newCameraTarget.transform.rotation = arrowsTransform.rotation * additionalRotation;
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = newCameraTarget.transform; // HACK leaves "CameraTarget" gameobject hanging in scene
            
            OnFinished?.Invoke(this, EventArgs.Empty);
        }     
        
        public void StartRun()
        {          
            _follower.SetSpeed(35);
            _follower.StartMovement();
        }
    }
}
