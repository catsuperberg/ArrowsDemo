using Sequence;
using System.Numerics;
using UnityEngine;

namespace GamePlay
{
    public interface IGamePlayManager : IFinishNotification
    {        
        public GameObject ActiveProjectile {get;}
        public void StartFromBeginning(GameObject level, SequenceContext context);
        // public void SetPause(bool toPause);
    }
}