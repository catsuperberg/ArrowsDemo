using Sequence;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using Utils;
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
        private List<GameObject> _arrows = new List<GameObject>();
        private int maxArrows = 50;
        
        
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
            fillArrows();
        }
        
        void fillArrows()
        {
            if(Count < maxArrows)
            {
                var arrowsToAdd = (int)Count - _arrows.Count;
                if(arrowsToAdd > 0)
                    AddArrows(arrowsToAdd);
                else
                {
                    for(int i = 0; i < arrowsToAdd; i++)
                        Destroy(_arrows[_arrows.Count - (1 + arrowsToAdd)]);
                }
            }
            else
            {
                if(_arrows.Count < maxArrows)
                {
                    var arrowsToAdd = maxArrows - _arrows.Count;
                    if(arrowsToAdd > 0)
                        AddArrows(arrowsToAdd);
                }   
                else
                {
                    for(int i = 0; i < _arrows.Count; i++)
                        _arrows[i].transform.localPosition = GetPositionOnSpiral(i);
                }             
            }
        }
        
        Vector3 GetPositionOnSpiral(int index)
        {
            var gridCoords = MathUtils.GetPositionOnSpiralGrid(index);
            var position = new Vector3(gridCoords.Item1*0.7f*Random.Range(0.7f, 1.3f),gridCoords.Item2*0.7f*Random.Range(0.7f, 1.3f), 0);
            return position;
        }
        
        void AddArrows(int countToAdd)
        {
            for(int i = 0; i < countToAdd; i++)
            {
                var arrow = Instantiate(ArrowAsset, GetPositionOnSpiral(_arrows.Count), UnityEngine.Quaternion.identity);
                arrow.transform.SetParent(transform, false);
                _arrows.Add(arrow);
            }
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

