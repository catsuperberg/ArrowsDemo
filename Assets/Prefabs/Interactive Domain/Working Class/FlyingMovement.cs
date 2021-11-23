using UnityEngine;

namespace GamePlay
{
    public class FlyingMovement : MonoBehaviour
    {
        float _speedRight = 0.1f;
        float _speedUp = 0.25f;
        float _speedFront = 0.04f;
        
        float _rangeRight = 0.2f;
        float _rangeUp = 0.4f;
        float _rangeFront = 0.9f;
        
        float _baseZ;
        
        void Awake()
        {
            _speedRight = Random.Range(2f, 4f);
            _speedUp    = Random.Range(0.5f, 1.8f);
            _speedFront = Random.Range(0.25f, 0.5f);
            
            _rangeRight = Random.Range(0.1f, 0.25f);
            _rangeUp    = Random.Range(0.2f, 0.9f);
            _rangeFront = Random.Range(1.2f, 4.0f);
            _baseZ = transform.localPosition.z + _rangeFront;
        }
        
        void Update()
        {
            transform.localPosition = new Vector3(Mathf.Sin(Time.time*_speedRight)*_rangeRight, 
                Mathf.Sin(Time.time*_speedUp)*_rangeUp, 
                _baseZ + Mathf.Sin(Time.time*_speedFront)*_rangeFront);
        }
    }
}