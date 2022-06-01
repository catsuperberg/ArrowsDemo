using System;

namespace Game.Microinteracions
{
    public class MicrointeractionEventArgs : EventArgs
    {
        public readonly IMicrointerationPacket Packet;
        
        public MicrointeractionEventArgs(IMicrointerationPacket packet)
        {
            Packet = packet;
        }
    }
}