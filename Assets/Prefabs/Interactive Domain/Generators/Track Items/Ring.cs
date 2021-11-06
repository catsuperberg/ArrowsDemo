using Sequence;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _operationText;
    
    private OperationInstance _operation;
    
    public void Initialize(OperationInstance newOperation)
    {
        _operation = newOperation;
        UpdateApearance();
    }
    
    void UpdateApearance()
    {
        _operationText.text = _operation.operationType.ToSymbol() + _operation.value;
    }
}
