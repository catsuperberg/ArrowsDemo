using Game.Gameplay.Runtime.Level.Track;
using Game.Gameplay.Runtime.OperationSequence.Operation;
using Game.Gameplay.Runtime.RunScene.GameCamera;
using Game.Gameplay.Runtime.RunScene.Projectiles;
using Input.ControllerComponents;
using SplineMesh;
using System;
using UnityEngine;

namespace Game.Gameplay.Runtime.RunScene.States
{
    public class GamePlayManager : IGamePlayManager
    {
        ITrackFollower _follower;    
        IProjectileProvider _projectileGenerator;  
        
        public event EventHandler OnFinished;
        public GameObject ActiveProjectile {get; private set;} = null;
        
        public GamePlayManager(ITrackFollower follower, IProjectileProvider projectileGenerator)
        {
            if(follower == null)
                throw new System.Exception("ITrackFollower not provided to GameManager");
            if(projectileGenerator == null)
                throw new System.Exception("ITrackFollower not provided to GameManager");
                
            _follower = follower;
            _projectileGenerator = projectileGenerator;
            
            _follower.OnFinished += GamePlayFinished;
        }
        
        
        void GamePlayFinished(object sender, EventArgs e)
        {
            var movementController2 = ActiveProjectile.GetComponent<TouchTranslationMovementController>(); // HACK i think what class is used for contrller should be defined in composition root
            if(movementController2 != null)
                GameObject.Destroy(movementController2);  
                
            var movementController = ActiveProjectile.GetComponent<ButtonsMovementController>(); // HACK i think what class is used for contrller should be defined in composition root
            if(movementController != null)
                GameObject.Destroy(movementController);
                
            // ActiveProjectile.transform.SetParent(null);
            
            var newCameraTarget = new GameObject("CameraTarget");
            var arrowsTransform = ActiveProjectile.GetComponentInChildren<TMPro.TMP_Text>().gameObject.transform;
            newCameraTarget.transform.position = arrowsTransform.position + new UnityEngine.Vector3(0, 14, 6);
            var additionalRotation =  UnityEngine.Quaternion.Euler(25, 0, 0);
            newCameraTarget.transform.rotation = arrowsTransform.rotation * additionalRotation;
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = newCameraTarget.transform;
            
            
            OnFinished?.Invoke(this, EventArgs.Empty);
        }     
             
        public void InitialiseRun(GameObject level, SequenceContext context)
        {
            if(ActiveProjectile != null)
            {                
                foreach(Transform child in ActiveProjectile.transform)
                    GameObject.Destroy(child.gameObject);
                GameObject.Destroy(ActiveProjectile);
            }
            
            var spline = level.GetComponentInChildren<Spline>();
            if(spline != null)
            {                
                _follower.SetSplineToFollow(spline, 0);
                var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
                smoothCamera.target = _follower.Transform;
                                
                ActiveProjectile = _projectileGenerator.CreateArrows(context.InitialValue, 12f); // HACK arbitrary width used for movement width
                ActiveProjectile.transform.SetParent(_follower.Transform);
                
                var movementController = ActiveProjectile.AddComponent<ButtonsMovementController>(); // HACK i think what class is used for contrller should be defined in construction root
                movementController.Init();
                var movementController2 = ActiveProjectile.AddComponent<TouchTranslationMovementController>();
                movementController2.Init();
            }
            else
                Debug.LogWarning("Illegalass GameObject provided as track in GamePlayManager.StartFromBeginning(GameObject level)");
        }
        
        public void StartRun()
        {          
            _follower.SetSpeed(35);
            _follower.StartMovement();
        }
    }
}