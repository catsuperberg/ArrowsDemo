using UnityEngine;

namespace GamePlay
{    
    public interface IMovable
    {
        public Vector3 Position {get;}
        public void moveRight(float distance);
        public void moveUp(float distance);
        public void moveForward(float distance);
        public void moveTo(Vector3 position);
    }
}