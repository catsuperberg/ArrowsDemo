using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Microinteracions
{
    public class RingAnimationInteractor : MonoBehaviour
    {
        [SerializeField]
        Animator RingAnimator;
        [SerializeField]
        string OnCorrectTriggerName;
        [SerializeField]
        string OnWrongTriggerName;
        
        void Awake()
        {
            var activators = gameObject.GetComponents<IMicrointerationActivator>();
            foreach(var activator in activators)
                activator.OnMicrointerationTriggered += PerformIfCorrectMicrointeration;
        }
    
        void PerformIfCorrectMicrointeration(object caller, MicrointeractionEventArgs args)
        {
            if(!(args.Packet is RingAnimationPacket))
                return;
                
            var packet = (RingAnimationPacket)args.Packet;
            var triggerName = (packet.CorrectChoice) ? OnCorrectTriggerName : OnWrongTriggerName;
            RingAnimator.SetTrigger(triggerName);
        }
    }
}
