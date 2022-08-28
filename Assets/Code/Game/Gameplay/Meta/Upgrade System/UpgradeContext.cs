using DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Meta.UpgradeSystem
{
    public class UpgradeContext : IConfigurable
    {
        [StoredField]
        public int CrossbowLevel {get; private set;} = 1;
        [StoredField]
        public int ArrowLevel {get; private set;} = 1;
        [StoredField]
        public int InitialArrowCount {get; private set;} = 1;
        [StoredField]
        public int PassiveIncome {get; private set;} = 1;  
                
        public event EventHandler OnUpdated;
                
        
        public UpgradeContext()
        {
                      
        }    
        
        public UpgradeContext(IRegistryIngester registry)
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
                case nameof(CrossbowLevel):
                    CrossbowLevel = Convert.ToInt32(fieldValue);
                    break;
                case nameof(ArrowLevel):
                    ArrowLevel = Convert.ToInt32(fieldValue);
                    break;
                case nameof(InitialArrowCount):
                    InitialArrowCount = Convert.ToInt32(fieldValue);
                    break;
                case nameof(PassiveIncome):
                    PassiveIncome = Convert.ToInt32(fieldValue);
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }
}