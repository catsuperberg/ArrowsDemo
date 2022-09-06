using System;
using System.Collections.Generic;

namespace DataManagement
{
    public abstract class Configurable : IConfigurable
    {
        public event EventHandler OnUpdated;
        public virtual void UpdateField(string fieldName, string fieldValue)
        {            
            SetFieldValue(fieldName, fieldValue);
            OnUpdated?.Invoke(this, EventArgs.Empty);   
        }
            
        public virtual void UpdateFields(List<(string fieldName, string fieldValue)> updatedValues)
        {            
            if(updatedValues.Count == 0)
                throw new System.Exception("No field data in array provided to UpdateFields function of class: " + this.GetType().Name);
            
            foreach(var fieldData in updatedValues)
                SetFieldValue(fieldData.fieldName, fieldData.fieldValue);       
                
            OnUpdated?.Invoke(this, EventArgs.Empty);    
        }
        
        internal abstract void SetFieldValue(string fieldName, string fieldValue);
    }  
}