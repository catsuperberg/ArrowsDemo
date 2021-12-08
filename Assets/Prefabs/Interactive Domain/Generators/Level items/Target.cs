using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using GamePlay;

namespace Level
{
    namespace Target
    {
        public class Target : MonoBehaviour, IDamageable
        {    
            [SerializeField]
            public int BasePoints;
            
            public BigInteger Points {get; private set;}
            public BigInteger DamagePoints {get {return Points;}}
            public TargetGrades Grade  {get; private set;}
            
            public void Initialize(BigInteger points, TargetGrades grade, float scaleCoeff)
            {
                Points = points;
                Grade = grade;
                
                GetComponent<Renderer>().material = grade.Material();
                
                var scale = gameObject.transform.localScale*scaleCoeff;
                gameObject.transform.localScale = scale;
            }                
                        
            public void Damage(BigInteger value)
            {                
                if(value > Points)
                    throw new System.Exception("Triying to damage one target more than possible");
                
                Points -= value;
            }
        }
    }
}
