using Sequence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace GamePlay
{
    public class GameManager
    {
        IMetaGame _meta;
        
        public GameManager(IMetaGame meta)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to GameManager");
                        
            _meta = meta;
            
            SequenceContext context = new SequenceContext(100, 5, 15);
            BigInteger targetResult = new BigInteger(0);
            
            targetResult = _meta.GetAverageSequenceResult(context, 150);
            Debug.Log("Targer result is: " + targetResult);
            
            var spread = 15;
            var sequence = _meta.GenerateSequence(targetResult, spread, context);
            
            foreach(OperationPair pair in sequence.Sequence)
            {
                var leftOperation = Enum.GetName(typeof(Operations), pair.LeftOperation.operationType);
                var rightOperation = Enum.GetName(typeof(Operations), pair.RighOperation.operationType);
                Debug.Log("left operation: " + leftOperation + ":" + pair.LeftOperation.value +  
                    "\t right operation" + rightOperation + ":" + pair.RighOperation.value);
            }
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