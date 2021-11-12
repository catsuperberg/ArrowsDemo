using Sequence;
using System.Numerics;
using UnityEngine;

namespace Level
{
    public interface ILevelManager
    {
        public GameObject InitializeLevel(SequenceContext context, OperationPairsSequence sequence, BigInteger targetResult);
    }
}
    