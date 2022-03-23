namespace Game.GameState
{
    public enum AppState
    {   
        PreRun,
        Runthrough,
        PostRun,
        Ad
    }
        
    public enum SubState
    {   
        Generation,
        PauseMenu,
        Blank
    }
        
    public enum ProcessState
    {   
        Blank               = 1,
        Processing          = 1 << 1,
        ResultAvailable     = 1 << 2,
        Finished            = 1 << 3
    }
    
    static class ProcessStateMethods
    {
        public static ProcessState SetFlag(this ProcessState enumContainer, ProcessState state)
        {
            return enumContainer | state;
        }
        
        public static ProcessState ClearFlag(this ProcessState enumContainer, ProcessState state)
        {
            var tempEnum = enumContainer;
            tempEnum &= ~state;  
            return tempEnum;          
        }
    }
}