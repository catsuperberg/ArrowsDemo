namespace Game.GameState
{
    public interface IStateSignal
    {
        public void SendStartGame();
        public void SendPauseMenu();
        public void SendDebugMenu();
        public void SendPreviousState();
    } 
}