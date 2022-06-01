using System;
using UnityEngine;

namespace Game.Gameplay.Realtime.OperationSequence.Operation
{
    public class OperationPairComponent : MonoBehaviour
    {
        public OperationPair Pair {get; private set;}
        bool initialized = false;
        
        public void Initialize(OperationPair pair)
        {
            if(initialized)
                throw new Exception("Gate pair is already initialized");
            
            initialized = true;
            Pair = pair;
        }
    }
}