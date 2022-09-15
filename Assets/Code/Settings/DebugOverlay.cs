using DataManagement;
using System;
using UnityEngine;
using Zenject;

namespace Settings
{    
    public class DebugOverlay : Configurable
    {               
        [StoredField("Debug Overlay")]
        public bool OverlayEnabled {get; private set;} = false;
        
        GameObject _overlayInScene;

        public DebugOverlay([Inject(Id = "settingsIngester")] IRegistryIngester registry,[Inject(Id = "DebugOverlay")] GameObject overlayInScene)
        {
            registry.Register(this, true, true);
            _overlayInScene = overlayInScene ?? throw new ArgumentNullException(nameof(overlayInScene));
        }


        internal override void SetFieldValue(string fieldName, string fieldValue)
        {
            switch(fieldName)
            {
                case nameof(OverlayEnabled):
                    OverlayEnabled = Convert.ToBoolean(fieldValue);
                    ApplyValue();
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
        
        void ApplyValue()
        {           
            UnityMainThreadDispatcher.Instance().Enqueue(() =>{    
                _overlayInScene.SetActive(OverlayEnabled);});
        }
    }
}