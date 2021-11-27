using Sequence;
using System.Numerics;
using UnityEngine;

namespace GamePlay
{
    public interface IGamePlayManager
    {
        public void StartFromBeginning(GameObject level, SequenceContext context);
        // public void SetPause(bool toPause);
    }
}