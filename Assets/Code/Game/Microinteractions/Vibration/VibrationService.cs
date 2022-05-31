using DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Microinteracions
{
    public class VibrationService : IConfigurable
    {        
        [StoredField("Vibration")]
        public bool VibrationEnabled {get; private set;} = true; 
        
        public event EventHandler OnUpdated;
        
        public VibrationService([Inject(Id = "settingsIngester")] IRegistryIngester registry)
        {
            registry.Register(this, true, true);  
            
            Vibration.Init();        
        }
        
        public void AffirmativeVibration()
        {
            Vibration.VibratePeek();
        } 
        
        public void NegativeVibration()
        {            
            Vibration.VibrateNope();
        }
        
        public void SmallVibration()
        {
            Vibration.VibratePop();            
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
                case nameof(VibrationEnabled):
                    VibrationEnabled = Convert.ToBoolean(fieldValue);
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }    
    
}