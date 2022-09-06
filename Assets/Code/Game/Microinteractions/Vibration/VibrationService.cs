using DataManagement;
using System;
using System.Collections.Generic;
using Zenject;

namespace Game.Microinteracions
{
    public class VibrationService : Configurable
    {        
        [StoredField("Vibration")]
        public bool VibrationEnabled {get; private set;} = true; 
        
        public VibrationService([Inject(Id = "settingsIngester")] IRegistryIngester registry)
        {
            registry.Register(this, true, true);  
            
            Vibration.Init();        
        }
        
        public void ExecuteEffect(VibrationEffect effect)
        {
            switch (effect)
            {
                case VibrationEffect.Affirmative:
                    AffirmativeVibration();
                    break;
                case VibrationEffect.Negative:
                    NegativeVibration();
                    break;
                case VibrationEffect.SmallVibration:
                    SmallVibration();
                    break;
            }
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
            
        internal override void SetFieldValue(string fieldName, string fieldValue)
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