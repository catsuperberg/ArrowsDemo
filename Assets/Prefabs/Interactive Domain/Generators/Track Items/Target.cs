using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Level
{
    namespace Track
    {
        namespace Target
        {
            public class Target : MonoBehaviour
            {    
                [SerializeField]
                public int BasePoints;
                
                public BigInteger Points {get; private set;}
                public TargetGrades Grade  {get; private set;}
                
                public void Initialize(BigInteger points, TargetGrades grade, float scaleCoeff)
                {
                    Points = points;
                    Grade = grade;
                    
                    GetComponent<Renderer>().material = grade.Material();
                    
                    var scale = gameObject.transform.localScale*scaleCoeff;
                    gameObject.transform.localScale = scale;
                }                
            }
        }
    }
}
