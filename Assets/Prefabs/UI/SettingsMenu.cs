using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameSettings
{
    public class SettingsMenu : MonoBehaviour
    {
        private ISettingsService _settingsService;
        private AudioVisualOptions _optionToSet = (AudioVisualOptions)(-1);
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
        
        public void SetEnumValue(UiUtils.EnumSwitcherHelper enumHandler)
        {
            int value = enumHandler.CurrentPreset;
            SetIntValue(value);
        }
        
        private void ExecuteOptionForValue<T>(T value)
        {
            if(validToExecute(value.GetType(), OptionsHelper.OptionType(_optionToSet)))
                if(value is int) _settingsService.SetOption<int>(_optionToSet, Convert.ToInt32(value));
                else if(value is float) _settingsService.SetOption<float>(_optionToSet, Convert.ToSingle(value));
                else if(value is bool) _settingsService.SetOption<bool>(_optionToSet, Convert.ToBoolean(value));
                else if(value is string) _settingsService.SetOption<string>(_optionToSet, Convert.ToString(value));
            else
                optionDiscardedWarning();
            resetOption();
        }  
        
        private bool validToExecute(System.Type optionType, System.Type valueType)
        {
            if(_optionsQueued == 1 && Enum.IsDefined(typeof(AudioVisualOptions), _optionToSet) && optionType == valueType)
                return true;
            else
                return false;
        }
        
        private void resetOption()
        {            
            _optionToSet = (AudioVisualOptions)(-1);
            _optionsQueued = 0;            
        }
        
        private void optionDiscardedWarning()
        {
            Debug.LogWarning("Settings execution was stopped, _optionsRegistered = " + _optionsQueued + 
                " _optionToSet: " + Enum.GetName(typeof(AudioVisualOptions), _optionToSet));
        }
    }
}