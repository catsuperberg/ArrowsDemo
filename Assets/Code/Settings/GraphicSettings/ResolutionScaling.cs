using DataManagement;
using Game.Gameplay.Realtime.GameplayComponents.GameCamera;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Settings
{
    public class ResolutionScaling : IConfigurable
    {               
        [StoredField("Resolution scaling", 0.2f, 1f)]
        public float Scaling {get; private set;} = 1;        
                 
        public event EventHandler OnUpdated;
        
        public ResolutionScaling([Inject(Id = "settingsIngester")] IRegistryIngester registry)
        {
            registry.Register(this, true, true);   
        }
        
        void SetScaling()
        {
            var scaler = Camera.main.GetComponent<ResolutionScaler>();
            if(scaler != null)
                scaler.SetScale(Convert.ToSingle(Scaling));
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
                case nameof(Scaling):
                    Scaling = Convert.ToSingle(fieldValue);
                    SetScaling();
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }
}