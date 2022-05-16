using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.GameState;
using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading.Tasks;

namespace Game.Gameplay.Realtime.OperationSequence
{
    public class SequenceManager : ISequenceManager//, IStateReportableProcess
    {        
        ISequenceCalculator _meta;
        IContextProvider _runContextProvider;
        
        // public ProcessState State {get; private set;} = ProcessState.Blank;
        public event EventHandler<ProcessStateEventArgs> OnStateChanged;   
        
        const int _numIterationsForAverage = 2000;
        // SequenceContext _context = new SequenceContext(1000, 35, 35); // TEMP should be caculated from user data wich chould be loaded from disk
        // int _targetsListSize = 5;
        // ConcurrentQueue<BigInteger> _nextTargets = new ConcurrentQueue<BigInteger>();
        // Task _fillingTargets = null;
                
        public SequenceManager(ISequenceCalculator meta, IContextProvider runContextProvider)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to MetaManager");
                
            if(runContextProvider == null)
                throw new System.Exception("IContextProvider isn't provided to RuntimeFactory");
                
            _runContextProvider = runContextProvider;                
            _meta = meta;
            
            // _fillingTargets = Task.Run(() => {FillTargets();});
        }
        
        // public SequenceContext GetContext()
        // {
        //     return _runContextProvider.GetContext();
        // }
        
        public BigInteger GetNextTargetScore()
        {       
            // var target = PopTarget();   
            // if(target == -1)            
            // {               
            //     if (!_fillingTargets.Status.Equals(TaskStatus.Running))
            //         _fillingTargets = Task.Run(() => {FillTargets();});
            //     while(target == -1)
            //     {
            //         target = PopTarget();
            //     }
            // }
            var target = GenerateAverageTarget();
                                                   
            return target;
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread)
        {
            // State = State.SetFlag(ProcessState.Processing); 
            // OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
            
            var sequence = _meta.GenerateSequence(targetScore, spread, _runContextProvider.GetContext());
            
            // if(!_fillingTargets.Status.Equals(TaskStatus.Running))
            // {
            //     State = State.ClearFlag(ProcessState.Processing); 
            //     OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State));                 
            // }
            
            return sequence;
        }
        
        // void FillTargets()
        // {
        //     State = State.SetFlag(ProcessState.Processing);
        //     OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
                        
        //     while(_nextTargets.Count < _targetsListSize)
        //         PushTarget(GenerateAverageTarget()); 
            
        //     State = State.SetFlag(ProcessState.ResultAvailable);
        //     State = State.ClearFlag(ProcessState.Processing);
        //     OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
        // }
        
        BigInteger GenerateAverageTarget()
        {
            return _meta.GetAverageSequenceResult(_runContextProvider.GetContext(), _numIterationsForAverage);
        }
        
        // void PushTarget(BigInteger target)
        // {
        //     _nextTargets.Enqueue(target);
        //     if(!State.HasFlag(ProcessState.ResultAvailable))
        //     {
        //         State = State.SetFlag(ProcessState.ResultAvailable);
        //         OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State));                 
        //     }
        // }
        
        // BigInteger PopTarget()
        // {            
        //     BigInteger target; 
        //     var success = _nextTargets.TryDequeue(out target);
        //     if(success)
        //     {
        //         State = State.ClearFlag(ProcessState.ResultAvailable);
        //         OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
        //         return target;
        //     }
        //     else
        //         return -1; // should be NaN but big integer doesn't have it
        // }
    }
}