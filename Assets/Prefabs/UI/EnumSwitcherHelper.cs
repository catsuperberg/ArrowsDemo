using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UiUtils
{
    public class EnumSwitcherHelper : MonoBehaviour
    {        
        // IEnumSelectable _enum;
        
        [SerializeField]
        private GameObject CurrentStateOverlay;
        
        [SerializeField]
        public GameSettings.GraphicsPresets CurrentPreset;
                
        private int _enumSize;
        
        void Start()
        {
            _enumSize = Enum.GetValues(typeof(GameSettings.GraphicsPresets)).Length;
        }
        
        public void SwitchToNextOption()
        {
            if((int)CurrentPreset < _enumSize-1)
                CurrentPreset++;
            else
                CurrentPreset = 0;
            UpdateCurrentOverlay();            
        }
        
        public void UpdateCurrentOverlay()
        {
            var textField = CurrentStateOverlay.GetComponent<TMP_Text>();
            textField.text = GameSettings.OptionsHelper.GraphicsPresetName(CurrentPreset);
        }
        
    }
}
