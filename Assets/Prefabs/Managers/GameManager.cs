using Level;
using GamePlay;
using Sequence;
using SplineMesh;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace GamePlay
{
    public class GameManager
    {
        IMetaGame _meta;
        ILevelManager _levelManager;        
        ITrackFollower _follower;
        
        public GameManager(IMetaGame meta, ILevelManager levelManager, ITrackFollower follower)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to GameManager");
            if(levelManager == null)
                throw new System.Exception("ILevelManager not provided to GameManager");
            if(levelManager == null)
                throw new System.Exception("ITrackFollower not provided to GameManager");
                        
            _meta = meta;
            _levelManager = levelManager;
            _follower = follower;
            
            // TEMP generation test
            Debug.Log("Start time is: " + Time.realtimeSinceStartup);
            
            SequenceContext context = new SequenceContext(640, 2, 25);
            BigInteger targetResult = new BigInteger(0);
            
            targetResult = _meta.GetAverageSequenceResult(context, 250);
            Debug.Log("Targer result is: " + targetResult);
            Debug.Log("Average score generated at: " + Time.realtimeSinceStartup);
            
            var spread = 15;
            var sequence = _meta.GenerateSequence(targetResult, spread, context);
            Debug.Log("Sequence generated at: " + Time.realtimeSinceStartup);
            
            foreach(OperationPair pair in sequence.Sequence)
            {
                var leftOperation = Enum.GetName(typeof(Operations), pair.LeftOperation.operationType);
                var rightOperation = Enum.GetName(typeof(Operations), pair.RightOperation.operationType);
            }
            
            // TEMP level manager test
            var level = _levelManager.InitializeLevel(context, sequence, targetResult);
            Debug.Log("Level initialized at: " + Time.realtimeSinceStartup);
            
            // TEMP track follower test
            _follower.SetSplineToFollow(level.GetComponentInChildren<Spline>(), 0);
            _follower.SetSpeed(60);
            _follower.StartMovement();
        }
        // ILevelManager _levelManager;
        
        // void BeginRun()
        // {
            
        // }
        
        // void UpdateLevel()
        // {
        //     _levelManager.UpdateLevel(newContext);
        // }
    }
}