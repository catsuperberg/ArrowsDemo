using UnityEngine;

namespace Game.Gameplay.Runtime.RunScene.GameCamera
{
    [ExecuteInEditMode]
    public class SmoothFollow : MonoBehaviour
    { 
        public Transform target;
        
        [SerializeField]
        private float smoothSpeed = 0.6f;
        [SerializeField]
        private float angleSpeedCoefficient = 1f;   
        [SerializeField]
        private Vector3 Offset;
        [SerializeField]
        private Vector3 Rotation;
        private Vector3 Velocity = Vector3.zero;
        
        void Start()
        {
            transform.SetParent(null);
        }
        
        void Update()
        {
            if(target != null)
            {
                var desiredPosition = target.position + Offset;
                var smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref Velocity, smoothSpeed);
                transform.position = smoothedPosition;
                var desiredRotation =  target.rotation * Quaternion.Euler(Rotation);
                transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation,
                    Time.deltaTime * (1/smoothSpeed)*angleSpeedCoefficient); 
            }
        }
    }
}
