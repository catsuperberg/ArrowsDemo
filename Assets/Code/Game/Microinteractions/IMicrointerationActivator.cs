using System;

namespace Game.Microinteracions
{
    public interface IMicrointerationActivator
    {    
        public event EventHandler<MicrointeractionEventArgs> OnMicrointerationTriggered;
    }
}
