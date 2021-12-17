using Level;
using GamePlay;
using Sequence;
using SplineMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using ExtensionMethods;

namespace GamePlay
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
            var movementController = ActiveProjectile.GetComponent<ButtonsMovementController>(); // HACK i think what class is used for contrller should be defined in composition root
            if(movementController != null)
                GameObject.Destroy(movementController);
                
            var movementController2 = ActiveProjectile.GetComponent<TouchTranslationMovementController>(); // HACK i think what class is used for contrller should be defined in composition root
            if(movementController2 != null)
                GameObject.Destroy(movementController);
                
            ActiveProjectile.transform.SetParent(null);
            
            var newCameraTarget = new GameObject("CameraTarget");
            var arrowsTransform = ActiveProjectile.GetComponentInChildren<TMPro.TMP_Text>().gameObject.transform;
            newCameraTarget.transform.position = arrowsTransform.position + new UnityEngine.Vector3(0, 14, 6);
            var additionalRotation =  UnityEngine.Quaternion.Euler(25, 0, 0);
            newCameraTarget.transform.rotation = arrowsTransform.rotation * additionalRotation;
            var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
            smoothCamera.target = newCameraTarget.transform;
            
            
            OnFinished?.Invoke(this, EventArgs.Empty);
        }     
             
        public void StartFromBeginning(GameObject level, SequenceContext context)
        {
            var spline = level.GetComponentInChildren<Spline>();
            if(spline != null)
            {                
                _follower.SetSplineToFollow(spline, 0);
                var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
                smoothCamera.target = _follower.Transform;
                
                if(ActiveProjectile != null)
                    GameObject.Destroy(ActiveProjectile);
                
                ActiveProjectile = _projectileGenerator.CreateArrows(context.InitialValue, 12f); // HACK arbitrary width used for movement width
                ActiveProjectile.transform.SetParent(_follower.Transform);
                
                var movementController = ActiveProjectile.AddComponent<ButtonsMovementController>(); // HACK i think what class is used for contrller should be defined in construction root
                movementController.Init();
                var movementController2 = ActiveProjectile.AddComponent<TouchTranslationMovementController>();
                movementController2.Init();
                
                _follower.SetSpeed(35);
                _follower.StartMovement();
            }
            else
                Debug.LogWarning("Illegalass GameObject provided as track in GamePlayManager.StartFromBeginning(GameObject level)");
        }
    }
}