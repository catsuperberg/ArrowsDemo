using Game.Gameplay.Realtime.OperationSequence.Operation;
using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents
{
    public interface ILevelManager
    {
        public GameObject SplineTrack {get;}
        public GameObject Gates {get;}
        public GameObject Targets {get;}
        public GameObject Level {get;}
        
        public GameObject InitializeLevel(SequenceContext context, OperationPairsSequence sequence, BigInteger targetScore);
    }
}
    