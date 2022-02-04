using Game.Gameplay.Realtime.OperationSequence.Operation;
using UnityEngine;

namespace Game.Gameplay.Realtime.GameplayComponents
{
    public interface IRunSceneManager : IFinishNotification
    {        
        public GameObject ActiveProjectile {get;}
        public void InitialiseRun(GameObject level, SequenceContext context);
        public void StartRun();
    }
}