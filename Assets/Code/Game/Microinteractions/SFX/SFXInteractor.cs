using System;
using UnityEngine;
using Zenject;

namespace Game.Microinteracions
{
    public class SFXInteractor : MonoBehaviour
    {
        SFXService _sfx;
        
        void Awake()
        {
            var activators = gameObject.GetComponents<IMicrointerationActivator>();
            foreach(var activator in activators)
                activator.OnMicrointerationTriggered += PerformIfCorrectMicrointeration;
        }
        
        [Inject]
        public void Construct(SFXService sfx)
        {                
            _sfx = sfx ?? throw new ArgumentNullException(nameof(sfx)); 
        }
    
        void PerformIfCorrectMicrointeration(object caller, MicrointeractionEventArgs args)
        {       
            if(_sfx == null || !(args.Packet is SFXPacket))
                return;
                     
            var packet = args.Packet as SFXPacket;
            _sfx.ExecuteEffect(packet.EffectToTrigger);
        }
    }
}
