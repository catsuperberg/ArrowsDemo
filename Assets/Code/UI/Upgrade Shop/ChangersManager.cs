using DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{    
    public class ChangersManager : MonoBehaviour
    {
        [SerializeField]
        GameObject ItemBuyerPrefab;
        
        List<ItemBuyer> _changers = new List<ItemBuyer>();
               
        public void CreateChangerForValue(IRegistryAccessor registryAccessor, Type objectClass, string fieldName)
        {                        
            var changerGo = Instantiate(ItemBuyerPrefab, Vector3.zero, Quaternion.identity);
            changerGo.transform.SetParent(transform);
            var changer = changerGo.GetComponent<ItemBuyer>();
            changer.AttachToValue(registryAccessor, objectClass, fieldName);
            _changers.Add(changer);
        }
        
        public void refreshAllValues()
        {
            foreach(var changer in _changers)
            {
                changer.updateValueText();
            }
        }
    }    
}