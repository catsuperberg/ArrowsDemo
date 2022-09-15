using DataManagement;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{    
    public class SettingsButton : MonoBehaviour
    {
        [SerializeField]
        TMP_Text ButtonText;
        
        ICallable _callableObject;
                
        public void AttachToValue(string prettyName, ICallable callableObject)
        {
            _callableObject = callableObject ?? throw new ArgumentNullException(nameof(callableObject));
            ButtonText.text = prettyName;
        }
        
        public void ExecuteCollable()
        {            
            if(_callableObject == null) throw new ArgumentNullException(nameof(_callableObject));
            _callableObject?.Call();
        }
    }
}