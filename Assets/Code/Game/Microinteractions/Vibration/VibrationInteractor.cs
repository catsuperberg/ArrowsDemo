using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Microinteracions
{
    public class VibrationInteractor : MonoBehaviour
    {
        VibrationService _vibrator;
        
        void Awake()
        {
            var activators = gameObject.GetComponents<IMicrointerationActivator>();
            foreach(var activator in activators)
                activator.OnMicrointerationTriggered += PerformIfCorrectMicrointeration;
        }
        
        [Inject]
        public void Construct(VibrationService vibrator)
        {
            if(vibrator == null)
                throw new ArgumentNullException("VibrationService not provided to " + this.GetType().Name);
                
            _vibrator = vibrator; 
        }
    
        void PerformIfCorrectMicrointeration(object caller, MicrointeractionEventArgs args)
        {
            if(_vibrator == null || !_vibrator.VibrationEnabled || !(args.Packet is VibrationPacket))
                return;
                
            var packet = args.Packet as VibrationPacket;
            _vibrator.ExecuteEffect(packet.EffectToTrigger);
        }
    }
}
