using System;
using System.Collections.Generic;

namespace State
{
    public class StateEventArgs : EventArgs
    {
        public StateEventArgs(AppState state, HashSet<SubState> subStates)
        {
            State = state;
            SubStates = subStates;
        }

        public AppState State { get; set; }
        public HashSet<SubState> SubStates { get; set; }
    }
    
    public interface IStateChangeNotifier
    {
            public event EventHandler<StateEventArgs> OnStateChanged;    
    } 
}
