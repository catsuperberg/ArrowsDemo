using SplineMesh;

namespace GamePlay
{
    public interface ITrackFollower
    {
        public void SetSplineToFollow(Spline spline, float startingPoint);
        public void MoveToLength(float newPosition);
        public void SetSpeed(float speed);
        public void StartMovement();
        public void ToggleMovement();
        public void PauseMovement();
    }
}