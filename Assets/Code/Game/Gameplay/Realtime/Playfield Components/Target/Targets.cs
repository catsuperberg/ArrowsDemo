using Game.Gameplay.Realtime.GameplayComponents;
using Game.Gameplay.Realtime.GeneralUseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using ExtensionMethods;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Target
{
    public class Targets : MonoBehaviour, IGameObjectFillable, ITargetGroup, IDamageableWithTransforms, IMultiplierEventNotifier
    {
        [SerializeField]
        private TMP_Text _CountIndicator;
        
        public BigInteger Count {get ; private set;}
        public BigInteger DamagePoints {get {return Count;}}
        public List<GameObject> TargetObjects {get;} = new List<GameObject>();      
        
        public Transform MainTransform {get {return gameObject.transform;}}
        public List<Transform> ChildrenTransforms {get {return TargetObjects.Select(go => go.transform).ToList();}}
                
        public event EventHandler<MultiplierEventArgs> OnMultiplierEvent;
        
        public void AddObjectToList(GameObject objectToAdd)
        {
            TargetObjects.Add(objectToAdd);
            objectToAdd.transform.SetParent(transform);
            
            UpdateCount();
            UpdateAppearance();
        }
        
        public void Damage(BigInteger value)
        {
            if(value > Count)
                throw new System.Exception("Triying to damage targets more than possible");
                
            var damageCount = value;
            var targets = GetTargetComponents();
            
            foreach(Target target in targets)
            {
                if(target.Points <= damageCount)
                {
                    damageCount -= target.Points;
                    TargetObjects.Remove(target.gameObject);
                    Destroy(target.gameObject);
                    OnMultiplierEvent?.Invoke(this, new MultiplierEventArgs(){Multiplier = target.Grade.RewardMultiplier()});
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
            var targets = GetTargetComponents();
            
            var sum = new BigInteger(0);
            foreach(Target target in targets)
                sum += target.Points;
            
            return sum;
        }            
        
        void UpdateAppearance()
        {
            // _CountIndicator.text = Count.ToString("N0");
            _CountIndicator.text = Count.ParseToReadable();
        }
        
        void UpdateCount()
        {
            Count = SumOfTargetCounts();
        }
        
        List<Target> GetTargetComponents()
        {
            var targetScripts = new List<Target>();
            foreach(GameObject target in TargetObjects)
            {
                var tempScript = target.GetComponent<Target>();
                if(tempScript != null)
                    targetScripts.Add(tempScript);
            }
            return targetScripts;
        }
    }
}