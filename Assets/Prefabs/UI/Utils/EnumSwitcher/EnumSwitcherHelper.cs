using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace UiUtils
{
    public class EnumSwitcherHelper : MonoBehaviour
    {        
        [SerializeReference]
        public IEnumSelectable SelectableEnumImplementation;
        
        [SerializeField]
        private GameObject CurrentStateOverlay;
        
        [SerializeField]
        public int CurrentPreset;
        
        public void SwitchToNextOption()
        {
            if((int)CurrentPreset < SelectableEnumImplementation.Length()-1)
                CurrentPreset++;
            else
                CurrentPreset = 0;
            UpdateCurrentOverlay();            
        }
        
        public void UpdateCurrentOverlay()
        {
            var textField = CurrentStateOverlay.GetComponent<TMP_Text>();
            textField.text = SelectableEnumImplementation.ValueName(CurrentPreset);
        }
        
    }
}