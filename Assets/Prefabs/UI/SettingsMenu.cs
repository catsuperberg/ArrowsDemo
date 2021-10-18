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
        private int _optionsRegistered = 0;
        
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
            _optionsRegistered++;
        }
        
        public void SetIntValue(int value)
        {
            if(validToExecute(value.GetType(), OptionsHelper.OptionType(_optionToSet)))
                _settingsService.SetOption<int>(_optionToSet, value);
            else
                optionDiscardedWarning();
            resetOption();
        }
        
        public void SetEnumValue(UiUtils.EnumSwitcherHelper enumData)
        {
            int value = enumData.CurrentPreset;
            SetIntValue(value);
        }
        
        public void SetFloatValue(float value)
        {
            if(validToExecute(value.GetType(), OptionsHelper.OptionType(_optionToSet)))
                _settingsService.SetOption<float>(_optionToSet, value);
            else
                optionDiscardedWarning();
            resetOption();
        }
        
        public void SetBoolValue(bool value)
        {
            if(validToExecute(value.GetType(), OptionsHelper.OptionType(_optionToSet)))
                _settingsService.SetOption<bool>(_optionToSet, value);
            else
                optionDiscardedWarning();
            resetOption();
        }
        
        public void SetStringValue(string value)
        {
            if(validToExecute(value.GetType(), OptionsHelper.OptionType(_optionToSet)))
                _settingsService.SetOption<string>(_optionToSet, value);
            else
                optionDiscardedWarning();
            resetOption();
        }
        
        private bool validToExecute(System.Type optionType, System.Type valueType)
        {
            if(_optionsRegistered == 1 && Enum.IsDefined(typeof(AudioVisualOptions), _optionToSet) && 
                optionType == valueType)
                return true;
            else
                return false;
        }
        
        private void resetOption()
        {            
            _optionToSet = (AudioVisualOptions)(-1);
            _optionsRegistered = 0;            
        }
        
        private void optionDiscardedWarning()
        {
            Debug.LogWarning("Settings execution was stopped, _optionsRegistered = " + _optionsRegistered + 
                " _optionToSet: " + Enum.GetName(typeof(AudioVisualOptions), _optionToSet));
        }
    }
}