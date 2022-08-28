using DataManagement;
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
        
        IRegistryAccessor _settingsAccessor;        
        
        [Inject]
        public void Initialize([Inject(Id = "settingsAccessor")] IRegistryAccessor registryAccessor)
        {
            if(registryAccessor == null)
                throw new System.Exception("IRegistryAccessor isn't provided to " + this.GetType().Name);
                                
            _settingsAccessor = registryAccessor; 
            CreateSettingChangers();  
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
    }
}