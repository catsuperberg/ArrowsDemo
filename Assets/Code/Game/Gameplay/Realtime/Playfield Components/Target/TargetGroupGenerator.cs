using ExtensionMethods;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using GameMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private const int fractionalCorrectionThreshold = 300;
        private Random _rand = new Random(Guid.NewGuid().GetHashCode() + DateTime.Now.GetHashCode());
                
        public async Task<GameObject> GetSuitableTargetAsync(List<GameObject> targetPrefabs, 
            BigInteger targetResult, (int Min, int Max) numberOfTargetsRange)
        {
            _target = null;
            _targetPrefabs = targetPrefabs;
            _targetResult = targetResult;
            _numberOfTargets = _rand.Next(numberOfTargetsRange.Min, numberOfTargetsRange.Max);
            _targetScores = generateTargetScores(_targetResult, _numberOfTargets);  
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
            var targetGroup = Instantiate(_targetGroupPrefab, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
            targetGroup.name = "Targets";
            targetGroup.transform.SetParent(null);            
            
            var targets = targetGroup.GetComponent<IGameObjectFillable>();            
            foreach(BigInteger score in _targetScores)
            {
                var randomPrefab = _targetPrefabs[_rand.Next(0,_targetPrefabs.Count)];
                targets.AddObjectToList(CreateTargetFromPrefab(randomPrefab, score));           
                if(Time.deltaTime >= 0.01)
                   yield return null;
            }            
            
            _target = targetGroup;     
            _targetsChildren = targets.TargetObjects;     
            semaphore.Release();  
        }
        
        IEnumerator TargetPlacementCoroutine(SemaphoreSlim semaphore)
        {   
            var targetsArea = GameObjectUtils.GetAreaOfObjectsWithColliders(_targetsChildren)*1.5;
            var radius = (float)System.Math.Sqrt(targetsArea/System.Math.PI);
            
            if(GameObjectUtils.GetAreaOfObjectsWithColliders(_targetsChildren) >= System.Math.PI*System.Math.Pow(radius,2))
                throw new System.Exception("area of circle is smaller than area of occupied by objects in TargetGroupGenerator.PlaceInsideCircle");
            
            var tempRadius = radius;
            var entriesPending = _targetsChildren.Count;
            var hits = 0;
            while(entriesPending > 0)
            {
                var locationOnCircle = UnityEngine.Random.insideUnitCircle * tempRadius;
                var position = new UnityEngine.Vector3(locationOnCircle.x, 0, locationOnCircle.y);
                _targetsChildren[entriesPending-1].transform.position = position;
                _targetsChildren[entriesPending-1].transform.rotation = UnityEngine.Quaternion.Euler(0, _rand.Next(-150,-30), 0);
                Physics.SyncTransforms();
                
                var thisCollider = _targetsChildren[entriesPending-1].GetComponent<CapsuleCollider>();
                bool collisions = false;
                foreach(GameObject entry in _targetsChildren)
                {
                    var entryCollision = entry.GetComponent<CapsuleCollider>();
                    if(entryCollision != thisCollider && thisCollider.bounds.Intersects(entryCollision.bounds)) 
                    {                           
                        collisions = true; 
                        hits++;                             
                        if(hits >= 80) // OPTIMIZATION_POINT
                        {
                            hits = 0;
                            tempRadius *= 1.4f;
                        }                   
                        break;
                    }                
                    if(Time.deltaTime >= 0.01)
                        yield return null;             
                }
                if(!collisions)
                    entriesPending--;         
            }   
            
            
            foreach(GameObject entry in _targetsChildren)
                entry.transform.SetParent(_target.transform, false);
            
            semaphore.Release();
        }
        
        
        void PlaceInsideCircle(List<GameObject> objectsWithCollision, float circleRadius)
        {
            if(GameObjectUtils.GetAreaOfObjectsWithColliders(objectsWithCollision) >= System.Math.PI*System.Math.Pow(circleRadius,2))
                throw new System.Exception("area of circle is smaller than area of occupied by objects in TargetGroupGenerator.PlaceInsideCircle");
            
            var tempRadius = circleRadius;
            var entriesPending = objectsWithCollision.Count;
            var hits = 0;
            while(entriesPending > 0)
            {
                var locationOnCircle = UnityEngine.Random.insideUnitCircle * tempRadius;
                var position = new UnityEngine.Vector3(locationOnCircle.x, 0, locationOnCircle.y);
                objectsWithCollision[entriesPending-1].transform.position = position;
                objectsWithCollision[entriesPending-1].transform.rotation = UnityEngine.Quaternion.Euler(0, _rand.Next(-150,-30), 0);
                Physics.SyncTransforms();
                
                var thisCollider = objectsWithCollision[entriesPending-1].GetComponent<CapsuleCollider>();
                bool collisions = false;
                foreach(GameObject entry in objectsWithCollision)
                {
                    var entryCollision = entry.GetComponent<CapsuleCollider>();
                    if(entryCollision != thisCollider && thisCollider.bounds.Intersects(entryCollision.bounds)) 
                    {                           
                        collisions = true; 
                        hits++;                             
                        if(hits >= 80) // OPTIMIZATION_POINT
                        {
                            hits = 0;
                            tempRadius *= 1.4f;
                        }                   
                        break;
                    }                        
                }
                if(!collisions)
                    entriesPending--;         
            }                
        }
        
        public GameObject GetSuitableTarget(List<GameObject> targetPrefabs, BigInteger targetResult, (int Min, int Max) numberOfTargetsRange)
        {
            var numberOfTargets = _rand.Next(numberOfTargetsRange.Min, numberOfTargetsRange.Max);
            var targetScores = generateTargetScores(targetResult, numberOfTargets);              
            var targetGroup = Instantiate(_targetGroupPrefab, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
            targetGroup.name = "Targets";
            targetGroup.transform.SetParent(null);
            
            var targets = targetGroup.GetComponent<IGameObjectFillable>();
            foreach(BigInteger score in targetScores)
            {
                var randomPrefab = targetPrefabs[_rand.Next(0,targetPrefabs.Count)];
                targets.AddObjectToList(CreateTargetFromPrefab(randomPrefab, score));
            }
            var targetsArea = GameObjectUtils.GetAreaOfObjectsWithColliders(targets.TargetObjects)*1.5;
            var radius = (float)System.Math.Sqrt(targetsArea/System.Math.PI);
            PlaceInsideCircle(targets.TargetObjects, radius);
            
            foreach(GameObject entry in targets.TargetObjects)
                entry.transform.SetParent(targetGroup.transform, false);
            
            return targetGroup;
        }
        
        
        GameObject CreateTargetFromPrefab(GameObject prefab, BigInteger score)
        {
            var target = Instantiate(prefab, UnityEngine.Vector3.forward * 10000, UnityEngine.Quaternion.identity);
            var targetLogic = target.GetComponent<Target>();
            
            var randomGrade = (TargetGrades)_rand.Next(0, (int)TargetGrades.ENUM_END); // TODO probably should be weighted
            
            var instanceBaseScore = targetLogic.BasePoints*randomGrade.TargetDamagePointsMultiplier();
            var scaleCoeff = System.Math.Exp(BigInteger.Log(score) - BigInteger.Log(new BigInteger(instanceBaseScore)));
            var scale = Mathf.Clamp((float)scaleCoeff, 0.5f, 3f);
            
            targetLogic.Initialize(score, randomGrade, scale);
            
            return target;
        }
        
        List<BigInteger> generateTargetScores(BigInteger result, int size)
        {               
            var averageTargetScore = result/size;
            List<BigInteger> targetScores = GenerateRandomScores(size, averageTargetScore);             
            targetScores = CorrectTargetScores(targetScores, result);            
            return targetScores;
        }
        
        List<BigInteger> GenerateRandomScores(int size, BigInteger averageTargetScore)
        {
            var scores = new List<BigInteger>();
            for(int i = 0; i < size; i++)
            {
                var spread = averageTargetScore.multiplyByFraction(GlobalRandom.RandomDouble(0.2f, 0.85f)) * new BigInteger(MathUtils.RandomSign());
                var score = averageTargetScore+spread;
                scores.Add(score);
            }
            return scores;
        }        
        
        List<BigInteger> CorrectTargetScores(List<BigInteger> targetScores, BigInteger result)
        {
            var tempScores = targetScores;    
            var timeStarted = DateTimeOffset.UtcNow.ToUnixTimeSeconds();        
            while(SumOfTargets(tempScores) != result)
            {
                var correctionAmmount = result-SumOfTargets(tempScores);                    
                if(result >= fractionalCorrectionThreshold && correctionAmmount >= fractionalCorrectionThreshold)
                    tempScores = CorrectByFractioningOrMultiplying(tempScores, result); 
                else
                    tempScores = CorrectByAddingSubtracting(tempScores, result, correctionAmmount); 
                
                if(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timeStarted >= 40)
                    throw new TimeoutException("Target generation taking more than 40 seconds");
            }
            return tempScores;
        }
        
        List<BigInteger> CorrectByFractioningOrMultiplying(List<BigInteger> targetScores, BigInteger result)
        {       
            List<BigInteger> tempScores = new List<BigInteger>();           
            var resultDeviation = System.Math.Exp(BigInteger.Log(result) - BigInteger.Log(SumOfTargets(targetScores)));  
            foreach(BigInteger entry in targetScores)
            {
                var valueToAdd = entry.multiplyByFraction(resultDeviation);
                valueToAdd = (valueToAdd < 1) ? 1 : valueToAdd;
                tempScores.Add(entry.multiplyByFraction(resultDeviation));                
            }
            return tempScores;   
        }
        
        List<BigInteger> CorrectByAddingSubtracting(List<BigInteger> targetScores, BigInteger result, BigInteger errorToCorrect)
        {
            List<BigInteger> tempScores = new List<BigInteger>();          
            foreach(var score in targetScores)  
            {
                if(errorToCorrect != 0)
                {
                    var scoreWithError = score + errorToCorrect;
                    if(scoreWithError >= 1)
                    {
                        errorToCorrect = 0;                            
                        tempScores.Add(scoreWithError);
                    }
                    else if(scoreWithError == 0)
                    {                        
                        errorToCorrect = -1;                            
                        tempScores.Add(1);
                    }
                    else
                    {
                        errorToCorrect = scoreWithError - 1;             
                        tempScores.Add(1);
                    }
                }                
                else
                    tempScores.Add(score);
            }
            return tempScores;   
        }
        
        BigInteger SumOfTargets(List<BigInteger> targetScores)
        {
            return targetScores.Aggregate((currentSum, item) => currentSum + item);
        }
    }
}