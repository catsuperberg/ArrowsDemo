using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using TMPro;
using GamePlay;

namespace Level
{
    namespace Target
    {
        public class Targets : MonoBehaviour, IGameObjectFillable, ITargetGroup
        {
            [SerializeField]
            private TMP_Text _CountIndicator;
            
            public BigInteger Count {get ; private set;}
            public BigInteger DamagePoints {get {return Count;}}
            public List<GameObject> Objects {get;} = new List<GameObject>();
            
            public void AddObjectToList(GameObject objectToAdd)
            {
                Objects.Add(objectToAdd);
                objectToAdd.transform.SetParent(transform);
                
                UpdateCount();
                UpdateAppearance();
            }
            
            public void Damage(BigInteger value)
            {
                Debug.Log("Targets count: " + Count);
                Debug.Log("Targets damage value: " + value);
                if(value > Count)
                    throw new System.Exception("Triying to damage targets more than possible");
                    
                var damageCount = value;
                var targets = GetTargetComponentss();
                
                foreach(Target target in targets)
                {
                    if(target.Points <= damageCount)
                    {
                        damageCount -= target.Points;
                        Objects.Remove(target.gameObject);
                        Destroy(target.gameObject);
                    }
                    else
                    {
                        target.Damage(damageCount);
                        damageCount = 0;
                    }
                    if(damageCount <= 0)
                        break;
                }
                
                UpdateCount();
                UpdateAppearance();
            }
            
            BigInteger SumOfTargetCounts()
            {
                var targets = GetTargetComponentss();
                
                var sum = new BigInteger(0);
                foreach(Target target in targets)
                    sum += target.Points;
                
                return sum;
            }            
            
            void UpdateAppearance()
            {
                _CountIndicator.text = Count.ToString();
            }
            
            void UpdateCount()
            {
                Count = SumOfTargetCounts();
            }
            
            List<Target> GetTargetComponentss()
            {
                var targetScripts = new List<Target>();
                foreach(GameObject target in Objects)
                {
                    var tempScript = target.GetComponent<Target>();
                    if(tempScript != null)
                        targetScripts.Add(tempScript);
                }
                return targetScripts;
            }
        }
    }
}