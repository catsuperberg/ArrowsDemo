using Level;
using GamePlay;
using Sequence;
using SplineMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using ExtensionMethods;

namespace GamePlay
{
    public class GameManager : Controls.IDebugActions
    {
        IMetaGame _meta;
        ILevelManager _levelManager;        
        ITrackFollower _follower;    
        IProjectileProvider _projectileGenerator;  
        
        int _targetsListSize = 5;
        List<BigInteger> _nextTargets = new List<BigInteger>();
        SequenceContext _context = new SequenceContext(640, 2, 25);
        GameObject _level = null;
        GameObject _activeProjectile = null;
        
        public GameManager(IMetaGame meta, ILevelManager levelManager, ITrackFollower follower,
            IProjectileProvider projectileGenerator)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to GameManager");
            if(levelManager == null)
                throw new System.Exception("ILevelManager not provided to GameManager");
            if(follower == null)
                throw new System.Exception("ITrackFollower not provided to GameManager");
            if(projectileGenerator == null)
                throw new System.Exception("ITrackFollower not provided to GameManager");
                        
            _meta = meta;
            _levelManager = levelManager;
            _follower = follower;
            _projectileGenerator = projectileGenerator;
            
            _nextTargets.Capacity = _targetsListSize;
                      
            // TEMP controols test
            var gameplayControlls = new Controls();
            gameplayControlls.Debug.Enable();
            gameplayControlls.Debug.SetCallbacks(this);
        }
             
        public void OnGenerateNewTarget(InputAction.CallbackContext context)
        {
            if(context.performed)
                GenerateTargetScore(); 
        }
        
        public void OnGenerateTrackForFirstTarget(InputAction.CallbackContext context)
        {     
            if(context.performed)
                GenerateTrack();
        }
        
        public void OnStartGame(InputAction.CallbackContext context)
        {
            if(context.performed)
                LaunchFromStart();
        }
        
        void GenerateTargetScore()
        {
            // TEMP generation test
            Debug.Log("Start time is: " + Time.realtimeSinceStartup);  
            var targetResult = new BigInteger(0);
            if(_nextTargets.Any())
            {
                    while(targetResult <= _nextTargets.Last() || targetResult >= _nextTargets.Last().multiplyByFraction(1.3))     
                    targetResult = _meta.GetAverageSequenceResult(_context, 250);
            }
            else
                targetResult = _meta.GetAverageSequenceResult(_context, 250);
            _nextTargets.Add(targetResult);     
            Debug.Log("Average score generated at: " + Time.realtimeSinceStartup);
            Debug.Log("Targets results are:");
            foreach(BigInteger target in _nextTargets)
            {
                Debug.Log(target);
            }  
        }
        
        void GenerateTrack()
        {
            if(_nextTargets.Any())
            {
                var targetResult = _nextTargets.First();
                _nextTargets.Remove(targetResult);
                var spread = 15;
                var sequence = _meta.GenerateSequence(targetResult, spread, _context);
                Debug.Log("Sequence generated at: " + Time.realtimeSinceStartup);
                
                // TEMP level manager test
                _level = _levelManager.InitializeLevel(_context, sequence, targetResult);
                Debug.Log("Level initialized at: " + Time.realtimeSinceStartup);
            }
            else
                Debug.Log("No targets to generate track");
        }
        
        void LaunchFromStart()
        {
            // TEMP track follower test
            if(_level != null)
            {                
                _follower.SetSplineToFollow(_level.GetComponentInChildren<Spline>(), 0);
                var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
                smoothCamera.target = _follower.Transform;
                
                if(_activeProjectile != null)
                    GameObject.Destroy(_activeProjectile);
                
                _activeProjectile = _projectileGenerator.CreateArrows(_context.InitialValue, 12f);
                _activeProjectile.transform.SetParent(_follower.Transform);
                
                _follower.SetSpeed(20);
                _follower.StartMovement();
            }
        }
    }
}