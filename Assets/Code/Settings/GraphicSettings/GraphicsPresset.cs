using DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Settings
{
    public  class GraphicOptionsGetter : IOptionsGetter
    {        
        public string[] Options {get {return QualitySettings.names;}}
    }
    
    public class GraphicsPresset : IConfigurable
    {               
        [StoredField("Graphics quality", OptionGetter: typeof(GraphicOptionsGetter))]
        public string CurrentQualityLevel {get; private set;} = QualitySettings.names[QualitySettings.GetQualityLevel()];       
                 
        public event EventHandler OnUpdated;
        
        public GraphicsPresset([Inject(Id = "settingsIngester")] IRegistryIngester registry)
        {
            registry.Register(this, true, true);   
        }
        
        void ApplyQuality()
        {
            if(CurrentQualityLevel != QualitySettings.names[QualitySettings.GetQualityLevel()])
                QualitySettings.SetQualityLevel(Array.IndexOf(QualitySettings.names, CurrentQualityLevel), true);
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
                case nameof(CurrentQualityLevel):
                    CurrentQualityLevel = Convert.ToString(fieldValue);
                    ApplyQuality();
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }
}