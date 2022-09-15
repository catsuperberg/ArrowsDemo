using DataManagement;
using Settings;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UI
{    
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField]
        GameObject FillablePanel;        
        [SerializeField]
        GameObject ToggleChangerPrefab;
        [SerializeField]
        GameObject SliderChangerPrefab;
        [SerializeField]
        GameObject DropDownChangerPrefab;
        [SerializeField]
        GameObject SettingsButton;
        
        IRegistryAccessor _settingsAccessor;   
        ProgressReset.Factory _resetScriptFactory;     
        
        [Inject]
        public void Initialize([Inject(Id = "settingsAccessor")] IRegistryAccessor registryAccessor, ProgressReset.Factory resetScriptFactory)
        {
            _settingsAccessor = registryAccessor ?? throw new ArgumentNullException(nameof(registryAccessor)); 
            _resetScriptFactory = resetScriptFactory ?? throw new ArgumentNullException(nameof(resetScriptFactory));
            CreateSettingChangers();  
            CreateProgressResetButton();
        }
        
        void CreateSettingChangers()
        {
            var classesToControl = _settingsAccessor.GetRegisteredClasses();
            
            foreach(var registeredClass in classesToControl)
            {
                var fieldsToControl = _settingsAccessor.GetRegisteredFields(registeredClass);
                foreach(var field in fieldsToControl)
                    CreateChangerForFieldOfType(registeredClass, field);
            }
        }
        
        void CreateChangerForFieldOfType(Type classType, string fieldName)
        {
            var fieldType = _settingsAccessor.GetFieldType(classType, fieldName);
            if(fieldType == typeof(float))
            {
                var changerGO = Instantiate(SliderChangerPrefab, Vector3.zero, Quaternion.identity, FillablePanel.transform);
                var changer = changerGO.GetComponentInChildren<SettingSliderChanger>();
                changer.AttachToValue(_settingsAccessor, classType, fieldName);
            }
            else if (fieldType == typeof(bool))
            {
                var changerGO = Instantiate(ToggleChangerPrefab, Vector3.zero, Quaternion.identity, FillablePanel.transform);
                var changer = changerGO.GetComponentInChildren<SettingToggleChanger>();
                changer.AttachToValue(_settingsAccessor, classType, fieldName);
            }
            else if (fieldType == typeof(string) && _settingsAccessor.GetFieldMetadata(classType, fieldName).ValidOptions != null)
            {
                var changerGO = Instantiate(DropDownChangerPrefab, Vector3.zero, Quaternion.identity, FillablePanel.transform);
                var changer = changerGO.GetComponentInChildren<SettingsDropDownChanger>();
                changer.AttachToValue(_settingsAccessor, classType, fieldName);
            }
        }
        
        void CreateProgressResetButton()
        {
            var buttonGO = Instantiate(SettingsButton, Vector3.zero, Quaternion.identity, FillablePanel.transform);
            var button = buttonGO.GetComponent<SettingsButton>();
            var resetScript = _resetScriptFactory.Create();
            resetScript.gameObject.transform.SetParent(buttonGO.transform);
            button.AttachToValue("RESET PLAYER PROGRESS", resetScript);
            
            // var containerWithCallable = ProgressResetFactory.GetProgressResetWithInjestion();
            // var progressReset = containerWithCallable.GetComponent<ICallable>();
            // var buttonGO = Instantiate(SettingsButton, Vector3.zero, Quaternion.identity, FillablePanel.transform);
            // containerWithCallable.transform.SetParent(buttonGO.transform);
            // var button = buttonGO.GetComponent<SettingsButton>();
            // button.AttachToValue("RESET PLAYER PROGRESS", progressReset);
        }
    }
}