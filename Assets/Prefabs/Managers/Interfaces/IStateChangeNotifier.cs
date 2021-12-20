using System;

namespace State
{
    public interface IStateChangeNotifier
    {
            public event EventHandler<StateEventArgs> OnStateChanged;    
    } 
}