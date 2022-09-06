using System;
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
            _vibrator = vibrator ?? throw new ArgumentNullException(nameof(vibrator)); 
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
