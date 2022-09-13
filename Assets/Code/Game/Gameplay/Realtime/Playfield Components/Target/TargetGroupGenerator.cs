using AssetScripts.Instantiation;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using GameMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

using Random = System.Random;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Target
{
    public class TargetGroupGenerator : MonoBehaviour, ITargetProvider 
    {
        [SerializeField]
        private GameObject _targetGroupPrefab;
        
        private GameObject _target;
        private List<GameObject> _targetsChildren;
        private List<GameObject> _targetPrefabs;
        private BigInteger _targetResult;
        private List<BigInteger> _targetScores;
        private int _numberOfTargets;
        private IInstatiator _assetInstatiator;
        private Random _rand = new Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());
                
        public async Task<GameObject> GetTargetAsync(List<GameObject> targetPrefabs, 
            BigInteger targetResult, (int Min, int Max) numberOfTargetsRange, IInstatiator assetInstatiator)
        {
            if(assetInstatiator == null)
                throw new ArgumentNullException("IInstatiator isn't provided to: " + this.GetType().Name);
            
            _assetInstatiator = assetInstatiator;
            _target = null;
            _targetPrefabs = targetPrefabs;
            _targetResult = targetResult;
            _numberOfTargets = _rand.Next(numberOfTargetsRange.Min, numberOfTargetsRange.Max);
            _targetScores = RandomBigIntListWithSetSum.Generate(_targetResult, _numberOfTargets, spreadDeviation: (0.2f, 0.85f));  
            var targetGenerationSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(TargetGenerationCoroutine(targetGenerationSemaphore));});
            await targetGenerationSemaphore.WaitAsync();    
            var targetPlacementSemaphore = new SemaphoreSlim(0, 1);
            UnityMainThreadDispatcher.Instance().Enqueue(() => {StartCoroutine(TargetPlacementCoroutine(targetPlacementSemaphore));});
            await targetPlacementSemaphore.WaitAsync();    
            return _target;
        }
        
        IEnumerator TargetGenerationCoroutine(SemaphoreSlim semaphore)
        {                   
            var targetGroup = _assetInstatiator.Instantiate(_targetGroupPrefab, name: "Targets");          
            
            var targets = targetGroup.GetComponent<IGameObjectFillable>();            
            foreach(BigInteger score in _targetScores)
            {
                var randomPrefab = _targetPrefabs[_rand.Next(0,_targetPrefabs.Count)];
                targets.AddObjectToList(CreateTargetFromPrefabHiden(randomPrefab, score));           
                if(Time.deltaTime >= 0.02)
                   yield return null;
            }            
            
            _target = targetGroup;     
            _targetsChildren = targets.TargetObjects;     
            semaphore.Release();  
        }
        
        IEnumerator TargetPlacementCoroutine(SemaphoreSlim semaphore)
        {   
            var tempRadius = RadiusToFitTargets();
            var entriesPending = _targetsChildren.Count;
            var tries = 0;
            while(entriesPending > 0)
            {
                PlaceRandomlyInCircle(_targetsChildren[entriesPending-1], tempRadius);
                Physics.SyncTransforms();                
                var currentCollider = _targetsChildren[entriesPending-1].GetComponent<CapsuleCollider>();
                bool collisions = false;
                foreach(GameObject entry in _targetsChildren)
                {
                    var entryCollision = entry.GetComponent<CapsuleCollider>();
                    if(entryCollision != currentCollider && currentCollider.bounds.Intersects(entryCollision.bounds)) 
                    {                           
                        collisions = true; 
                        tries++;                             
                        if(tries >= 80) // OPTIMIZATION_POINT waiting for more hits results in slower but tighter placement
                        {
                            tries = 0;
                            tempRadius *= 1.4f;
                        }                   
                        break;
                    }                
                    if(Time.deltaTime >= 0.02)
                        yield return null;             
                }
                if(!collisions)
                    entriesPending--;         
            }   
            
            foreach(GameObject entry in _targetsChildren)
                entry.transform.SetParent(_target.transform, false);
            
            semaphore.Release();
        }
        
        float RadiusToFitTargets()
        {
            var targetsArea = GameObjectUtils.GetAreaOfObjectsWithColliders(_targetsChildren)*1.5;
            return (float)System.Math.Sqrt(targetsArea/System.Math.PI);
        }
        
        void PlaceRandomlyInCircle(GameObject entity, float radius)
        {
            entity.transform.position = RandomPositionOnCircle(radius);
            entity.transform.rotation = RandomHorizontalRotation((-150,-30));
        }
        
        UnityEngine.Vector3 RandomPositionOnCircle(float radius)
        {
            var locationOnCircle = UnityEngine.Random.insideUnitCircle * radius;
            return new UnityEngine.Vector3(locationOnCircle.x, 0, locationOnCircle.y);
        } 
        
        UnityEngine.Quaternion RandomHorizontalRotation((int min, int max) angleRange)
        {
            return UnityEngine.Quaternion.Euler(0, _rand.Next(angleRange.min, angleRange.max), 0);
        } 
        
        GameObject CreateTargetFromPrefabHiden(GameObject prefab, BigInteger score)
        {            
            var target = _assetInstatiator.Instantiate(prefab, position: UnityEngine.Vector3.forward * 10000);
            var targetLogic = target.GetComponent<Target>();
            
            var randomGrade = (TargetGrades)_rand.Next(0, (int)TargetGrades.ENUM_END); // TODO should be weighted
            
            var instanceBaseScore = targetLogic.BasePoints*randomGrade.TargetDamagePointsMultiplier();
            var scaleCoeff = System.Math.Exp(BigInteger.Log(score) - BigInteger.Log(new BigInteger(instanceBaseScore)));
            var scale = Mathf.Clamp((float)scaleCoeff, 0.5f, 3f);
            
            targetLogic.Initialize(score, randomGrade, scale);
            
            return target;
        }        
    }
}



