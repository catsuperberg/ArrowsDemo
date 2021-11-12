using Level;
using Sequence;
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
        
        public GameManager(IMetaGame meta, ILevelManager levelManager)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to GameManager");
            if(levelManager == null)
                throw new System.Exception("ILevelManager not provided to GameManager");
                        
            _meta = meta;
            _levelManager = levelManager;
            
            //TEMP generation test
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
                // Debug.Log("left operation: " + leftOperation + ":" + pair.LeftOperation.value +  
                //     "\t right operation" + rightOperation + ":" + pair.RighOperation.value);
            }
            
            //TEMP level manager test
            _levelManager.InitializeLevel(context, sequence, targetResult);
            Debug.Log("Level initialized at: " + Time.realtimeSinceStartup);
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