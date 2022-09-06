using DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Settings
{
    public  class GraphicOptionsGetter : IOptionsGetter
    {        
        public string[] Options {get {return QualitySettings.names;}}
    }
    
    public class GraphicsPresset : Configurable
    {               
        [StoredField("Graphics quality", OptionGetter: typeof(GraphicOptionsGetter))]
        public string CurrentQualityLevel {get; private set;} = QualitySettings.names[QualitySettings.GetQualityLevel()];      
        
        public GraphicsPresset([Inject(Id = "settingsIngester")] IRegistryIngester registry)
        {
            registry.Register(this, true, true);   
        }
        
        void ApplyQuality()
        {           
            if(CurrentQualityLevel != QualitySettings.names[QualitySettings.GetQualityLevel()])
                UpdateQuality(); 
        }
        
        void UpdateQuality()
        {
            KeepRenderScale();
            QualitySettings.SetQualityLevel(Array.IndexOf(QualitySettings.names, CurrentQualityLevel), true);            
        }
        
        void KeepRenderScale()
        {            
            var currentResolutionScaling = CureentURP().renderScale; 
            UnityMainThreadDispatcher.Instance().Enqueue(() => {CureentURP().renderScale = currentResolutionScaling;}); // executes at the end of a frame when UpdateQuality() done changing URP asset
        }
        
        UniversalRenderPipelineAsset CureentURP()
        {
            return (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        }
        
        internal override void SetFieldValue(string fieldName, string fieldValue)
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