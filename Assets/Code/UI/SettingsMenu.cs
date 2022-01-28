using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Settings;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        private ISettingsService _settingsService;
        private Settings.Settings _optionToSet = (Settings.Settings)(-1);
        private int _optionsQueued = 0;
        
        [Inject]
        public void Construct(ISettingsService settingsServiceImplementation)
        {
             if(settingsServiceImplementation == null)
                throw new System.Exception("ISettingsService isn't provided to SettingsMenu");
                
            _settingsService = settingsServiceImplementation;
        }
        
        public void SetOptionToChange(InspectorOptionSelector optionContext)
        {
            _optionToSet = optionContext.OptionToSet;
            _optionsQueued++;
        }
        
        public void SetIntValue(int value)
        {
            ExecuteOptionForValue<int>(value);
        }
        
        public void SetFloatValue(float value)
        {
            ExecuteOptionForValue<float>(value);
        }
        
        public void SetBoolValue(bool value)
        {
            ExecuteOptionForValue<bool>(value);
        }
        
        public void SetStringValue(string value)
        {
            ExecuteOptionForValue<string>(value);
        }
        
        public void ApplySettings()
        {
            _settingsService.ApplySettings();
            _settingsService.SaveSettings();
        }
        
        #if UNITY_EDITOR
        public void SetEnumValue(UiUtils.EnumSwitcherHelper enumHandler)
        {
            int value = enumHandler.CurrentPreset;
            SetIntValue(value);
        }
        #endif
                        
        private void ExecuteOptionForValue<T>(T value) where T : IComparable
        {
            _settingsService.SetOption(_optionToSet, value);
            resetOption();
        }  
        
        private void resetOption()
        {
            _optionToSet = (Settings.Settings)(-1);
            _optionsQueued = 0;            
        }
    }
}