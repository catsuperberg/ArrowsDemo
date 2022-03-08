using DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Meta.Curencies
{
    public class CurenciesContext : IConfigurable
    {
        [StoredField]
        public int CommonCoins {get; private set;} = 0;
                
        public event EventHandler OnUpdated;
        
        public CurenciesContext(IRegistryIngester registry)
        {
            registry.Register(this, true, true);            
        }    
        
        public void UpdateField(string fieldName, string fieldValue)
        {            
            SetFieldValue(fieldName, fieldValue);
            
            OnUpdated?.Invoke(this, EventArgs.Empty);            
        }
        
        public void UpdateFields(List<(string fieldName, string fieldValue)> updatedValues)
        {            
            if(updatedValues.Count == 0)
                throw new System.Exception("No field data in array provided to UpdateFields function of class: " + this.GetType().Name);
            
            foreach(var fieldData in updatedValues)
                SetFieldValue(fieldData.fieldName, fieldData.fieldValue);
            
            OnUpdated?.Invoke(this, EventArgs.Empty);            
        }
        
        void SetFieldValue(string fieldName, string fieldValue)
        {
            switch(fieldName)
            {
                case nameof(CommonCoins):
                    CommonCoins = Convert.ToInt32(fieldValue);
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }
}