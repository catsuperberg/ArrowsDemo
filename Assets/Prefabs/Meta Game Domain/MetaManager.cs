using Sequence;
using State;
using System;
using System.Collections.Generic;
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
        SequenceContext _context = new SequenceContext(1000, 15, 35); // TEMP should be caculated from user data wich chould be loaded from disk
        int _targetsListSize = 5;
        List<BigInteger> _nextTargets = new List<BigInteger>();
        Task _fillingTargets = null;
                
        public MetaManager(IMetaGame meta)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to MetaManager");
                
            _meta = meta;
            
            _nextTargets.Capacity = _targetsListSize;
            _fillingTargets = Task.Run(() => {FillTargets();});
        }
        
        public SequenceContext GetContext()
        {
            return _context;
        }
        
        public BigInteger GetNextTargetScore()
        {
            Debug.LogWarning("GetNextTargetScore() called");                
            var target = PopTarget();   
            if(target == -1)
            {
                PushTarget(GenerateAverageTarget()); 
                target = PopTarget();
            }
            else
            {
                _fillingTargets = Task.Run(() => {FillTargets();});
            }
                                       
            Debug.Log("GetNextTargetScore() result is: " + target); 
            return target;
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread)
        {
            Debug.LogWarning("GenerateSequence() called"); 
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
            
            AddTargetsToMax();
            
            State = State.SetFlag(ProcessState.ResultAvailable);
            State = State.ClearFlag(ProcessState.Processing);
            OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State)); 
        }
        
        void AddTargetsToMax()
        {         
            var targetsToAdd = _targetsListSize - _nextTargets.Count; 
            if(targetsToAdd > 0)
            {               
                BigInteger tempTarget = new BigInteger(0);                 
                for(int i = 0; i < targetsToAdd-1; i++)
                {
                    tempTarget = GenerateAverageTarget();
                    UnityMainThreadDispatcher.Instance().Enqueue(() => PushTarget(tempTarget)); 
                }
                              
                UnityMainThreadDispatcher.Instance().Enqueue(() => 
                {              
                    foreach(var result in _nextTargets)
                        Debug.Log(result);    
                });
            }           
        }
        
        BigInteger GenerateAverageTarget()
        {
            return _meta.GetAverageSequenceResult(_context, _numIterationsForAverage);
        }
        
        void PushTarget(BigInteger target)
        {
            _nextTargets.Add(target);
            if(!State.HasFlag(ProcessState.ResultAvailable))
            {
                State = State.SetFlag(ProcessState.ResultAvailable);
                OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State));                 
            }
        }
        
        BigInteger PopTarget()
        {
            if(!_nextTargets.Any())
                return -1; // should be NaN but big integer doesn't have it
            
            var target = _nextTargets.First();
            _nextTargets.Remove(target);
            if(!_nextTargets.Any())
            {
                State = State.ClearFlag(ProcessState.ResultAvailable);
                OnStateChanged?.Invoke(this, new ProcessStateEventArgs(State));                 
            }
            return target;
        }
    }
}