namespace Game.GameState
{
    public interface IStateSignal
    {
        public void SendStartGame();
        public void SendUpgradeShop();
        public void SendPauseMenu();
        public void SendDebugMenu();
        public void SendPreviousState();
    } 
}