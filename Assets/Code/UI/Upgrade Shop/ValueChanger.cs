using DataManagement;
using System;
using TMPro;
using UnityEngine;

namespace UI
{    
    public class ValueChanger : MonoBehaviour
    {
        [SerializeField]
        TMP_Text NameText;
        [SerializeField]
        TMP_Text ValueText;
        
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
            
            NameText.text = _fieldName;
            updateValueText();            
        }
        
        public void IncreaseValue(string increment)
        {
            _registryAccessor.ApplyOperationOnRegisteredField(_objectClass, _fieldName, OperationType.Increase, increment);
            updateValueText();
        }
        
        public void DecreaseValue(string increment)
        {
            _registryAccessor.ApplyOperationOnRegisteredField(_objectClass, _fieldName, OperationType.Decrease, increment);    
            updateValueText();        
        }    
        
        public void updateValueText()
        {
            ValueText.text = _registryAccessor.GetStoredValue(_objectClass, _fieldName);
        }
    }
}