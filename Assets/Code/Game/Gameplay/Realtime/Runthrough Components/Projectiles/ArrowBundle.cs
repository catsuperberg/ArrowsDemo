using ExtensionMethods;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using GameMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;

using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Game.Gameplay.Realtime.GameplayComponents.Projectiles
{
    public class ArrowBundle : MonoBehaviour, IProjectile, IMovable, IDamageableWithTransforms
    {
        [SerializeField]
        public float MovementWidth {get; private set;}
        [SerializeField]
        public float UpwardOffset = 4;
        [SerializeField]
        private GameObject ArrowAsset;
        [SerializeField]
        private TMP_Text CountIndicator;
        [SerializeField]
        private Rigidbody Rigidbody;
        private List<GameObject> _arrows = new List<GameObject>();
        private int maxArrows = 70;
        
        public GameObject ProjectilePrefab {get {return ArrowAsset;}}        
        public Transform MainTransform {get {return gameObject.transform;}}
        public List<Transform> ChildrenTransforms {get {return _arrows.Select(go => go.transform).ToList();}}
        
        public BigInteger DamagePoints {get {return Count;}}
        public Vector3 Position {get {return transform.position;}}
        public BigInteger Count {get; private set;} = new BigInteger(1);
        public bool CollisionEnabled {get; private set;} = false;
        
        public event EventHandler OnUpdated;
        public bool Paused {get; private set;} = false;
        
        public GameObject GameObject {get {return gameObject;}}
                                        
        public void Initialize(BigInteger initialCount, float movementWidth, bool collisionEnabled)
        {
            Count = initialCount;
            MovementWidth = movementWidth;
            CollisionEnabled = collisionEnabled;
            UpdateAppearance();
            ClampPosition();
            // PreventFromInitialMovement();
        }
        
        // void PreventFromInitialMovement()
        // {
        //     Rigidbody.velocity = Vector3.zero;                    
        //     Rigidbody.angularVelocity = Vector3.zero;   
        //     Rigidbody. 
        // }
        
        public void EnableCollison()
        {
            CollisionEnabled = true;
        }
        
        public void DisableCollison()
        {
            CollisionEnabled = false;
        }
        
        void OnTriggerEnter(Collider collider)
        {
            if(!CollisionEnabled)
                return;
            IMathContainer MathContainer = collider.gameObject.GetComponent<IMathContainer>();
            if(MathContainer != null)
            {                
                Count = MathContainer.ApplyOperation(Count);
                UpdateAppearance();
            }
        }
        
        void UpdateAppearance()
        {            
            OnUpdated?.Invoke(this, EventArgs.Empty);
            CountIndicator.text = Count.ParseToReadable();
            fillArrows();
        }
        
        void fillArrows()
        {
            if(Count < maxArrows)
            {
                var arrowsToAdd = (int)Count - _arrows.Count;
                if(arrowsToAdd > 0)
                    AddArrows(arrowsToAdd);
                else if (arrowsToAdd < 0)
                {
                    var arrowsToDestroy = Math.Clamp(-arrowsToAdd, 0, _arrows.Count);
                    var arrowsAfterDestoy = _arrows.Count - arrowsToDestroy;
                    for(int i = _arrows.Count; i > arrowsAfterDestoy; i--)
                    {
                        var arrowToRemove = _arrows[i-1];
                        _arrows.Remove(arrowToRemove);
                        Destroy(arrowToRemove);                        
                    }
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
                    for(int i = 1; i < _arrows.Count-1; i++) // not from 0 as first arrow shouldn't change position
                        if(Random.Range(1, 5) >= 2)
                            _arrows[i].transform.localPosition = GetPositionOnSpiral(i);
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
                arrow.SetActive(true);
                arrow.transform.SetParent(transform, false);
                _arrows.Add(arrow);
            }
        }
                
        public void SetPaused(bool stateToSet)
        {
            Paused = stateToSet;
        }
        
        public void moveRight(float distance)
        {
            if(Paused)
                return;
            var position = transform.localPosition;
            position.x += distance;
            transform.localPosition = position;
            ClampPosition();
        }
        
        public void moveUp(float distance) {if(Paused) return;}
        public void moveForward(float distance) {if(Paused) return;}
        
        public void moveTo(Vector3 position)
        {         
            if(Paused)
                return;   
            var tempPosition = transform.localPosition;
            tempPosition.x += position.x;
            transform.localPosition = tempPosition;
            ClampPosition();
        }
        
        
        public void Damage(BigInteger value)
        {
            if(value > Count)
                throw new System.Exception("Triying to damage Arrow bundle more than possible");
                    
            Count -= value;
            if(Count < 1)
                Destroy(gameObject);
            UpdateAppearance();
        }
        
        void ClampPosition()
        {
            var position = transform.localPosition;
            var absRange = MovementWidth/2;
            if(Math.Abs(position.x) > absRange)
                position.x = Math.Sign(position.x) * absRange;   
            position.y = UpwardOffset;
            transform.localPosition = position;               
        }        
    }
}

