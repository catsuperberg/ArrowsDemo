using System;

namespace Game.Gameplay.Realtime.GameplayComponents
{    
    public class MultiplierEventArgs : EventArgs
    {
        public float Multiplier { get; set; }
    }
    
    public interface IMultiplierEventNotifier
    {
        public event EventHandler<MultiplierEventArgs> OnMultiplierEvent;
    }
}