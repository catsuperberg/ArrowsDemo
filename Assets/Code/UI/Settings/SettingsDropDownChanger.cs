using DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{    
    public class SettingsDropDownChanger : MonoBehaviour
    {
        [SerializeField]
        TMP_Text NameText;
        [SerializeField]
        TMP_Dropdown Dropdown;
        
        IRegistryAccessor _registryAccessor;
        Type _objectClass;
        string _fieldName;
                        
        public void AttachToValue(IRegistryAccessor registryAccessor, Type objectClass, string fieldName)
        {
            if(registryAccessor == null)
                throw new NullReferenceException("No IRegistryAccessor implimentation provided to: " + this.GetType().Name);
            
            _registryAccessor = registryAccessor;
            _objectClass = objectClass;
            _fieldName = fieldName;
            
            var metadata = _registryAccessor.GetFieldMetadata(_objectClass, _fieldName);  
            NameText.text = metadata.PrettyName;  
            InitializeDropdown();
        }
        
        void InitializeDropdown()
        {            
            var metadata = _registryAccessor.GetFieldMetadata(_objectClass, _fieldName);  
            Dropdown.ClearOptions();
            var dropdownOptions = metadata.ValidOptions.ToList();
            if(dropdownOptions != null)
                Dropdown.AddOptions(dropdownOptions);
            var currentValue = Convert.ToString(_registryAccessor.GetStoredValue(_objectClass, _fieldName));
            Dropdown.value = Dropdown.options.FindIndex(option => option.text == currentValue);
        }
        
        public void SetValue(int valueIndex)
        {
            var valueString = Dropdown.options[valueIndex].text;            
            _registryAccessor.ApplyOperationOnRegisteredField(_objectClass, _fieldName, OperationType.Replace, valueString);
            MakeSureValueDisplayedIsValueInRegistry();
        }        
        
        void MakeSureValueDisplayedIsValueInRegistry()
        {
            StartCoroutine(UpdateVisualiserAfter(Time.deltaTime*2));
        }
        
        IEnumerator UpdateVisualiserAfter(float time)
        {
            yield return new WaitForSeconds(time);
                        
            var currentValue = Convert.ToString(_registryAccessor.GetStoredValue(_objectClass, _fieldName));
            Dropdown.value = Dropdown.options.FindIndex(option => option.text == currentValue);
        }
    }
}