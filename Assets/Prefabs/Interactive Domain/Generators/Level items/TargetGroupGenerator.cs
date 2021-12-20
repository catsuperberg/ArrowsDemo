using ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Level.Target;
using UnityEngine;
using Utils;

namespace Level
{
    public class TargetGroupGenerator : MonoBehaviour, ITargerProvider 
    {
        [SerializeField]
        private GameObject _targetGroupPrefab;
        
        public GameObject GetSuitableTarget(List<GameObject> targetPrefabs, BigInteger targetResult, (int Min, int Max) numberOfTargetsRange)
        {
            var numberOfTargets = Random.Range(numberOfTargetsRange.Min, numberOfTargetsRange.Max);
            var targetScores = generateTargetScores(targetResult, numberOfTargets);              
            var targetGroup = Instantiate(_targetGroupPrefab, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
            targetGroup.name = "Targets";
            targetGroup.transform.SetParent(null);
            
            var targets = targetGroup.GetComponent<IGameObjectFillable>();
            foreach(BigInteger score in targetScores)
            {
                var randomPrefab = targetPrefabs[Random.Range(0,targetPrefabs.Count)];
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
            var targetLogic = target.GetComponent<Target.Target>();
            
            var randomGrade = (TargetGrades)Random.Range(0, (int)TargetGrades.ENUM_END);
            var instanceBaseScore = targetLogic.BasePoints*randomGrade.Multiplier();
            var scaleCoeff = System.Math.Exp(BigInteger.Log(score) - BigInteger.Log(new BigInteger(instanceBaseScore)));
            var scale = Mathf.Clamp((float)scaleCoeff, 0.5f, 3f);
            
            targetLogic.Initialize(score, randomGrade, scale);
            
            return target;
        }
        
        List<BigInteger> generateTargetScores(BigInteger result, int size)
        {
            List<BigInteger> targetScores = new List<BigInteger>();                
            var averageTargetScore = result/size;
            while(SumOfTargets(targetScores) != result)
            {
                var spread = averageTargetScore.multiplyByFraction(Random.Range(0.2f, 0.85f))  * new BigInteger(MathUtils.RandomSign());
                var score = averageTargetScore+spread;
                
                if(targetScores.Count-1 < size)
                    targetScores.Add(score);
                else
                    targetScores[Random.Range(0, size)] = score;
                    targetScores = TryCorrectingToResult(targetScores, result, 0.1);                      
            }
            return targetScores;
        }
        
        List<BigInteger> TryCorrectingToResult(List<BigInteger> targetScores, BigInteger result, double maxDeviation)
        {                  
            var resultDeviation = System.Math.Exp(BigInteger.Log(result) - BigInteger.Log(SumOfTargets(targetScores)));                
            if(System.Math.Abs(1 - resultDeviation) <= maxDeviation)
            {
                List<BigInteger> tempScores = new List<BigInteger>();
                foreach(BigInteger entry in targetScores)
                    tempScores.Add(entry.multiplyByFraction(resultDeviation));
                var error = result - SumOfTargets(tempScores);
                tempScores[tempScores.Count-1] = tempScores.Last() + error;
                return tempScores;
            }          
            else
                return targetScores;       
        }
        
        BigInteger SumOfTargets(List<BigInteger> targetScores)
        {
            BigInteger sum = new BigInteger(0);
            foreach(BigInteger score in targetScores)
                sum = BigInteger.Add(sum, score);
            return sum;
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
                var locationOnCircle = Random.insideUnitCircle * tempRadius;
                var position = new UnityEngine.Vector3(locationOnCircle.x, 0, locationOnCircle.y);
                // var rotation = new UnityEngine.Vector3(0, Random.Range(-150,30), 0);
                objectsWithCollision[entriesPending-1].transform.position = position;
                objectsWithCollision[entriesPending-1].transform.rotation = UnityEngine.Quaternion.Euler(0, Random.Range(-150,-30), 0);
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
                        if(hits >= 1000)
                        {
                            hits = 0;
                            tempRadius *= 1.2f;
                        }                   
                        break;
                    }                         
                }
                if(!collisions)
                    entriesPending--;         
            }                
        }
    }
}