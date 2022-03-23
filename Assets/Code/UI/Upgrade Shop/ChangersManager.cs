using DataManagement;
using System;
using UnityEngine;

namespace UI
{    
    public class ChangersManager : MonoBehaviour
    {
        [SerializeField]
        GameObject ValueChangerPrefab;
        
        public void CreateChangerForValue(IRegistryAccessor registryAccessor, Type objectClass, string fieldName)
        {                        
            var changer = Instantiate(ValueChangerPrefab, Vector3.zero, Quaternion.identity);
            changer.transform.SetParent(transform);
            changer.GetComponent<ValueChanger>().AttachToValue(registryAccessor, objectClass, fieldName);
        }
    }    
}