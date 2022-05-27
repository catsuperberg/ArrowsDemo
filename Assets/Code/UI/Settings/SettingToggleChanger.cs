using DataManagement;
using System;
using TMPro;
using System.Collections;
using UnityEngine;

namespace UI
{    
    public class SettingToggleChanger : MonoBehaviour
    {
        [SerializeField]
        TMP_Text NameText;
        [SerializeField]
        ToggleVisualizer Visualizer;
        
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
            
            
            if(NameText != null)
                NameText.text = _fieldName;    
        }
        
        public void Toggle()
        {
            var currentValue = _registryAccessor.GetStoredValue(_objectClass, _fieldName);            
            _registryAccessor.ApplyOperationOnRegisteredField(_objectClass, _fieldName, OperationType.Replace, PerformNotOnStringBool(currentValue));
        }        
        
        string PerformNotOnStringBool(string boolean)
        {
            return (boolean == true.ToString()) ? true.ToString() : false.ToString();
        }
        
        void UpdateVisualiser()
        {
            StartCoroutine(UpdateVisualiserAfter(Time.deltaTime*2));
        }
        
        IEnumerator UpdateVisualiserAfter(float time)
        {
            yield return new WaitForSeconds(time);
            
            var state = Convert.ToBoolean(_registryAccessor.GetStoredValue(_objectClass, _fieldName)); 
            Visualizer.SetState(state);
        }
    }
}