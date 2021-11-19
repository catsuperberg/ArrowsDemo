using Sequence;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using Zenject;

namespace Level
{
    namespace Track
    {
        namespace Items
        {
            public class Ring : MonoBehaviour, IMathContainer
            {
                [SerializeField]
                private TMP_Text _operationText;
                
                private OperationInstance _operation;
                private OperationExecutor _exec;
                
                public void Initialize(OperationInstance newOperation, OperationExecutor exec)
                {
                    if(exec == null)
                        throw new System.Exception("OperationExecutor not provided to Ring");
                    _exec = exec;                        
                    _operation = newOperation;
                    UpdateApearance();
                }
                
                void UpdateApearance()
                {
                    if(_exec == null)
                        throw new System.Exception("Ring wasn't initialized");
                    _operationText.text = _operation.operationType.ToSymbol() + _operation.value;
                }
                
                public BigInteger ApplyOperation(BigInteger initialValue)
                {
                    if(_exec == null)
                        throw new System.Exception("Ring wasn't initialized");
                    var newValue = _exec.Perform(_operation, initialValue);
                    return newValue;
                }
            }
        }
    }
}