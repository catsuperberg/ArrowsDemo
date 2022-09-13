using DataManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Settings
{
    public class ResolutionScaling : Configurable
    {               
        [StoredField("Resolution scaling", 0.2f, 1f)]
        public float Scaling {get; private set;} = 1;        
                         
        public ResolutionScaling([Inject(Id = "settingsIngester")] IRegistryIngester registry)
        {
            registry.Register(this, true, true);   
        }
        
        internal override void SetFieldValue(string fieldName, string fieldValue)
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
        
        void SetScaling()
        {            
            var urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                urp.renderScale = Convert.ToSingle(Scaling);
        }
    }
}