namespace Game.Gameplay.Realtime.GeneralUseInterfaces
{        
    public interface IPausable
    {
        public bool Paused {get;}        
        public void SetPaused(bool stateToSet);
    }
}