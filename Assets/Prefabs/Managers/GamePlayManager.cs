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
        // IMetaGame _meta;
        // ILevelManager _levelManager;        
        ITrackFollower _follower;    
        IProjectileProvider _projectileGenerator;  
        
        // int _targetsListSize = 5;
        // List<BigInteger> _nextTargets = new List<BigInteger>();
        // SequenceContext _context = new SequenceContext(640, 2, 25);
        // GameObject _level = null;
        GameObject _activeProjectile = null;
        
        public GamePlayManager(ITrackFollower follower, IProjectileProvider projectileGenerator)
        {
        // public GamePlayManager(IMetaGame meta, ILevelManager levelManager, ITrackFollower follower,
        //     IProjectileProvider projectileGenerator)
        // {
        //     if(meta == null)
        //         throw new System.Exception("IMetaGame not provided to GameManager");
        //     if(levelManager == null)
        //         throw new System.Exception("ILevelManager not provided to GameManager");
            if(follower == null)
                throw new System.Exception("ITrackFollower not provided to GameManager");
            if(projectileGenerator == null)
                throw new System.Exception("ITrackFollower not provided to GameManager");
                        
        //     _meta = meta;
        //     _levelManager = levelManager;
            _follower = follower;
            _projectileGenerator = projectileGenerator;
            
            // _nextTargets.Capacity = _targetsListSize;
        }
             
             
        public void StartFromBeginning(GameObject level, SequenceContext context)
        {
            var spline = level.GetComponentInChildren<Spline>();
            if(spline != null)
            {                
                _follower.SetSplineToFollow(spline, 0);
                var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
                smoothCamera.target = _follower.Transform;
                
                if(_activeProjectile != null)
                    GameObject.Destroy(_activeProjectile);
                
                _activeProjectile = _projectileGenerator.CreateArrows(context.InitialValue, 12f); // HACK arbitrary width used for movement width
                _activeProjectile.transform.SetParent(_follower.Transform);
                
                _follower.SetSpeed(35);
                _follower.StartMovement();
            }
            else
                Debug.LogWarning("Illegalass GameObject provided as track in GamePlayManager.StartFromBeginning(GameObject level)");
        }
        
        // public void OnGenerateNewTarget(InputAction.CallbackContext context)
        // {
        //     if(context.performed)
        //         GenerateTargetScore(); 
        // }
        
        // public void OnGenerateTrackForFirstTarget(InputAction.CallbackContext context)
        // {     
        //     if(context.performed)
        //         GenerateTrack();
        // }
        
        // public void OnStartGame(InputAction.CallbackContext context)
        // {
        //     if(context.performed)
        //         LaunchFromStart();
        // }
        
        // void GenerateTargetScore()
        // {
        //     // TEMP generation test
        //     Debug.Log("Start time is: " + Time.realtimeSinceStartup);  
        //     var targetResult = new BigInteger(0);
        //     if(_nextTargets.Any())
        //     {
        //             while(targetResult <= _nextTargets.Last() || targetResult >= _nextTargets.Last().multiplyByFraction(1.3))     
        //             targetResult = _meta.GetAverageSequenceResult(_context, 2500);
        //     }
        //     else
        //         targetResult = _meta.GetAverageSequenceResult(_context, 2500);
        //     _nextTargets.Add(targetResult);     
        //     Debug.Log("Average score generated at: " + Time.realtimeSinceStartup);
        //     Debug.Log("Targets results are:");
        //     foreach(BigInteger target in _nextTargets)
        //     {
        //         Debug.Log(target);
        //     }  
        // }
        
        // void GenerateTrack()
        // {
        //     if(_nextTargets.Any())
        //     {
        //         var targetResult = _nextTargets.First();
        //         _nextTargets.Remove(targetResult);
        //         var spread = 15;
        //         Debug.Log("Sequence generation started at: " + Time.realtimeSinceStartup);
        //         var sequence = _meta.GenerateSequence(targetResult, spread, _context);
        //         Debug.Log("Sequence generated at: " + Time.realtimeSinceStartup);
                
        //         // TEMP level manager test
        //         _level = _levelManager.InitializeLevel(_context, sequence, targetResult);
        //         Debug.Log("Level initialized at: " + Time.realtimeSinceStartup);
        //     }
        //     else
        //         Debug.Log("No targets to generate track");
        // }
        
        // void LaunchFromStart()
        // {
        //     // TEMP track follower test
        //     if(_level != null)
        //     {                
        //         _follower.SetSplineToFollow(_level.GetComponentInChildren<Spline>(), 0);
        //         var smoothCamera = Camera.main.GetComponent<SmoothFollow>();
        //         smoothCamera.target = _follower.Transform;
                
        //         if(_activeProjectile != null)
        //             GameObject.Destroy(_activeProjectile);
                
        //         _activeProjectile = _projectileGenerator.CreateArrows(_context.InitialValue, 12f);
        //         _activeProjectile.transform.SetParent(_follower.Transform);
                
        //         _follower.SetSpeed(20);
        //         _follower.StartMovement();
        //     }
        // }
    }
}