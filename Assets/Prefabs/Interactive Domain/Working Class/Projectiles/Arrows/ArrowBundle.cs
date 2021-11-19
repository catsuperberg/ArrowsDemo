using Sequence;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace GamePlay
{
    public class ArrowBundle : MonoBehaviour, IProjectileObject, IMovable
    {
        [SerializeField]
        public float MovementWidth;
        // public float MovementWidth {get{return MovementWidth;} set {MovementWidth = value; ClampPosition();}}
        [SerializeField]
        public float UpwardOffset = 4;
        [SerializeField]
        private GameObject ArrowAsset;
        [SerializeField]
        private TMP_Text _CountIndicator;
        
        
        public Vector3 Position {get {return transform.position;}}
        public BigInteger Count {get; private set;} = new BigInteger(1);
        
        
        public void Initialize(BigInteger initialCount, float movementWidth)
        {
            Count = initialCount;
            MovementWidth = movementWidth;
            UpdateAppearance();
            ClampPosition();
        }
        
        void OnTriggerEnter(Collider collider)
        {
            Debug.Log("collision");
            IMathContainer MathContainer = collider.gameObject.GetComponent<IMathContainer>();
            if(MathContainer != null)
            {
                Count = MathContainer.ApplyOperation(Count);
                UpdateAppearance();
            }
        }
        
        void UpdateAppearance()
        {
            _CountIndicator.text = Count.ToString();
        }
        
        public void moveRight(float distance)
        {
            var position = transform.localPosition;
            position.x += distance;
            transform.localPosition = position;
            ClampPosition();
        }
        
        public void moveUp(float distance) {}
        public void moveForward(float distance) {}
        
        public void moveTo(Vector3 position)
        {            
            var tempPosition = transform.localPosition;
            tempPosition.x += position.x;
            transform.localPosition = tempPosition;
            ClampPosition();
        }
        
        void ClampPosition()
        {
            var position = transform.localPosition;
            var absRange = MovementWidth/2;
            if(Mathf.Abs(position.x) > absRange)
                position.x = Mathf.Sign(position.x) * absRange;   
            position.y = UpwardOffset;
            transform.localPosition = position;               
        }        
    }
}

