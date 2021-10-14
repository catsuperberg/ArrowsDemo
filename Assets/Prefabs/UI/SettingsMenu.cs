using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameSettings
{
    public class SettingsMenu : MonoBehaviour
    {
        private ISettingsService _settingsService;
        private AudioVisualOptions _optionToSet;
        
        [Inject]
        public void Construct(ISettingsService settingsServiceImplementation)
        {
             if(settingsServiceImplementation == null)
                throw new System.Exception("IGameFolders isn't provided to Settings");
                
            _settingsService = settingsServiceImplementation;
        }
        
        public void SetOptionToChange(InspectorOptionSelector optionContext)
        {
            _optionToSet = optionContext.OptionToSet;
            // _settingsService.SetOption<float>(AudioVisualOptions.MasterVolume, value);
        }
        
        public void SetOptionValue(float value)
        {
            _settingsService.SetOption<float>(_optionToSet, value);            
        }
    }
}