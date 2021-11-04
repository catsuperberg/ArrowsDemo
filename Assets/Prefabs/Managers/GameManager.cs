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
            SequenceContext context = new SequenceContext(260, 5, 15);
            BigInteger targetResult = new BigInteger(0);
            
            targetResult = _meta.GetAverageSequenceResult(context, 150);
            Debug.Log("Targer result is: " + targetResult);
            
            var spread = 15;
            var sequence = _meta.GenerateSequence(targetResult, spread, context);
            
            foreach(OperationPair pair in sequence.Sequence)
            {
                var leftOperation = Enum.GetName(typeof(Operations), pair.LeftOperation.operationType);
                var rightOperation = Enum.GetName(typeof(Operations), pair.RighOperation.operationType);
                // Debug.Log("left operation: " + leftOperation + ":" + pair.LeftOperation.value +  
                //     "\t right operation" + rightOperation + ":" + pair.RighOperation.value);
            }
            
            //TEMP level manager test
            _levelManager.InitializeLevel(context, sequence);
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