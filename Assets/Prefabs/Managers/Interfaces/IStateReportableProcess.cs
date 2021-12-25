using System;

namespace State
{
    public class ProcessStateEventArgs : EventArgs
    {
        public ProcessStateEventArgs(ProcessState state)
        {
            State = state;
        }

        public ProcessState State { get; set; }
    }
    
    public interface IStateReportableProcess
    {
        public ProcessState State {get;}
        public event EventHandler<ProcessStateEventArgs> OnStateChanged;    
    } 
}