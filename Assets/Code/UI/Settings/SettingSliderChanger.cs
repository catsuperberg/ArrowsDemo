using DataManagement;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{    
    public class SettingSliderChanger : MonoBehaviour
    {
        [SerializeField]
        TMP_Text NameText;
        [SerializeField]
        Slider Slider;
        
        IRegistryAccessor _registryAccessor;
        Type _objectClass;
        string _fieldName;
        bool _sliderReadyToUpdateSettingOnSetValue = false;
                        
        public void AttachToValue(IRegistryAccessor registryAccessor, Type objectClass, string fieldName)
        {
            if(registryAccessor == null)
                throw new NullReferenceException("No IRegistryAccessor implimentation provided to: " + this.GetType().Name);
            
            _registryAccessor = registryAccessor;
            _objectClass = objectClass;
            _fieldName = fieldName;
            
            var metadata = _registryAccessor.GetFieldMetadata(_objectClass, _fieldName);  
            NameText.text = metadata.PrettyName;  
            InitializeSlider();
        }
        
        void InitializeSlider()
        {            
            var metadata = _registryAccessor.GetFieldMetadata(_objectClass, _fieldName);  
            Slider.maxValue = metadata.MaxValue; 
            Slider.minValue = metadata.MinValue;      
            var currentValue = Convert.ToSingle(_registryAccessor.GetStoredValue(_objectClass, _fieldName));
            Slider.SetValueWithoutNotify(currentValue);
            _sliderReadyToUpdateSettingOnSetValue = true;  
        }
        
        public void SetValue(float newValue)
        {
            if(!_sliderReadyToUpdateSettingOnSetValue)
                return;
            var valueString = newValue.ToString();            
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
                        
            var currentValue = Convert.ToSingle(_registryAccessor.GetStoredValue(_objectClass, _fieldName));
            if(currentValue != Slider.value)
                Slider.SetValueWithoutNotify(currentValue);
        }
    }
}