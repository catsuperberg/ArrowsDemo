using SplineMesh;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public interface ITrackFollower : IFinishNotification
    {
        public Transform Transform {get;}
        
        public void SetSplineToFollow(Spline spline, float startingPoint);
        public void MoveToLength(float newPosition);
        public void SetSpeed(float speed);
        public void StartMovement();
        public void ToggleMovement();
        public void PauseMovement();
    }
}