using Sequence;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using ExtensionMethods;

namespace GameMeta
{
    public class MetaManager : IMetaManager
    {        
        IMetaGame _meta;
        
        const int _numIterationsForAverage = 1600;
        SequenceContext _context = new SequenceContext(1000, 15, 35); // TEMP should be caculated from user data wich chould be loaded from disk
        int _targetsListSize = 5;
        List<BigInteger> _nextTargets = new List<BigInteger>();
        
        public MetaManager(IMetaGame meta)
        {
            if(meta == null)
                throw new System.Exception("IMetaGame not provided to MetaManager");
                
            _meta = meta;
            
            _nextTargets.Capacity = _targetsListSize;
            FillTargets();
        }
        
        public SequenceContext GetContext()
        {
            return _context;
        }
        
        public BigInteger GetNextTargetScore()
        {
            var target = _nextTargets.First();
            _nextTargets.Remove(_nextTargets.First());
            FillTargets();
            return target;
        }
        
        public OperationPairsSequence GenerateSequence(BigInteger targetScore, int spread)
        {
            return _meta.GenerateSequence(targetScore, spread, _context);
        }
        
        void FillTargets()
        {
            var startTime = Time.realtimeSinceStartup;
            Debug.Log("FillTargets() started at: " + Time.realtimeSinceStartup);
            
            if(!_nextTargets.Any())
                _nextTargets.Add(_meta.GetAverageSequenceResult(_context, _numIterationsForAverage));
            var targetsToAdd = _targetsListSize - _nextTargets.Count; 
            var targetResult = new BigInteger(0);
            if(targetsToAdd > 0)
            {
                for(int i = 0; i < targetsToAdd; i++)
                {               
                    // while(targetResult <= _nextTargets.Last() || targetResult >= _nextTargets.Last().multiplyByFraction(1.3))     
                    //     targetResult = _meta.GetAverageSequenceResult(_context, _numIterationsForAverage);
                    targetResult = _meta.GetAverageSequenceResult(_context, _numIterationsForAverage);
                    _nextTargets.Add(targetResult); 
                }
            }                
            foreach(BigInteger target in _nextTargets)
                Debug.Log(target);             
                
            Debug.Log("FillTargets() took: " + (Time.realtimeSinceStartup - startTime));
        }
    }
}