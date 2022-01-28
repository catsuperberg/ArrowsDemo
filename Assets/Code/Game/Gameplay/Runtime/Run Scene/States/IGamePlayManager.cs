using Game.Gameplay.Runtime.OperationSequence.Operation;
using UnityEngine;

namespace Game.Gameplay.Runtime.RunScene.States
{
    public interface IGamePlayManager : IFinishNotification
    {        
        public GameObject ActiveProjectile {get;}
        public void InitialiseRun(GameObject level, SequenceContext context);
        public void StartRun();
    }
}