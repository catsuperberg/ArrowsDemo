using Sequence;
using System.Numerics;
using UnityEngine;

namespace Level
{
    public interface ILevelManager
    {
        public GameObject SplineTrack {get;}
        public GameObject Gates {get;}
        public GameObject Targets {get;}
        
        public GameObject InitializeLevel(SequenceContext context, OperationPairsSequence sequence, BigInteger targetResult);
    }
}
    