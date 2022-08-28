using DataManagement;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Meta.Curencies
{
    public class CurenciesContext : IConfigurable
    {
        [StoredField]
        public BigInteger CommonCoins {get; private set;} = 0;
        [StoredField]
        public BigInteger SkinTokens {get; private set;} = 0;
        [StoredField]
        public BigInteger LifetimeSpending {get; private set;} = 0;
                
        public event EventHandler OnUpdated;
        
        public CurenciesContext()
        {
            
        }
        
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
                    CommonCoins = BigInteger.Parse(fieldValue);
                    break;
                case nameof(SkinTokens):
                    SkinTokens = BigInteger.Parse(fieldValue);
                    break;
                case nameof(LifetimeSpending):
                    LifetimeSpending = BigInteger.Parse(fieldValue);
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }
}