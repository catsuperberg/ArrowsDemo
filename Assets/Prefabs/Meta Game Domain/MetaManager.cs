using Sequence;
using State;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Numerics;
using UnityEngine;
using ExtensionMethods;

namespace GameMeta
{
    public class MetaManager : IMetaManager
    {        
        IMetaGame _meta;
        
        public ProcessState State {get; private set;} = ProcessState.Blank;
        public event EventHandler<ProcessStateEventArgs> OnStateChanged;   
        
        const int _numIterationsForAverage = 1600;
        SequenceContext _context = new SequenceContext(1000, 35, 35); // TEMP should be caculated from user data wich chould be loaded from disk
        int _targetsListSize = 5;
        ConcurrentQueue<BigInteger> _nextTargets = new ConcurrentQueue<BigInteger>();
        Task _fillingTargets = null;
                
        public MetaManager(IMetaGame meta)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to MetaManager");
                
            _meta = meta;
            
            _fillingTargets = Task.Run(() => {FillTargets();});
        }
        
        public SequenceContext GetContext()
        {
            return _context;
        }
        
        public BigInteger GetNextTargetScore()
        {       
            var target = PopTarget();   
            if(target == -1)            
            {               
                if (!_fillingTargets.Status.Equals(TaskStatus.Running))
                    _fillingTargets = Task.Run(() => {FillTargets();});
                while(target == -1)
                {
                    target = PopTarget();
                }
            }
                                                   
            return target;
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread)
        {
            State = State.SetFlag(ProcessState.Processing); 
            OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
            
            var sequence = _meta.GenerateSequence(targetScore, spread, _context);
            
            if(!_fillingTargets.Status.Equals(TaskStatus.Running))
            {
                State = State.ClearFlag(ProcessState.Processing); 
                OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State));                 
            }
            
            return sequence;
        }
        
        void FillTargets()
        {
            State = State.SetFlag(ProcessState.Processing);
            OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
                        
            while(_nextTargets.Count < _targetsListSize)
                PushTarget(GenerateAverageTarget()); 
            
            State = State.SetFlag(ProcessState.ResultAvailable);
            State = State.ClearFlag(ProcessState.Processing);
            OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
        }
        
        BigInteger GenerateAverageTarget()
        {
            return _meta.GetAverageSequenceResult(_context, _numIterationsForAverage);
        }
        
        void PushTarget(BigInteger target)
        {
            _nextTargets.Enqueue(target);
            if(!State.HasFlag(ProcessState.ResultAvailable))
            {
                State = State.SetFlag(ProcessState.ResultAvailable);
                OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State));                 
            }
        }
        
        BigInteger PopTarget()
        {            
            BigInteger target; 
            var success = _nextTargets.TryDequeue(out target);
            if(success)
            {
                State = State.ClearFlag(ProcessState.ResultAvailable);
                OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
                return target;
            }
            else
                return -1; // should be NaN but big integer doesn't have it
        }
    }
}