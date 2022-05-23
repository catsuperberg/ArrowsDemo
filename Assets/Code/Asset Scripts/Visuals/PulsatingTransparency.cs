using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetScripts.Visual
{
    public class PulsatingTransparency : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 0.5f;
        [SerializeField]
        private float _range = 0.3f;
        
        private Renderer _rend;
        private float _baseTransparency;
        
        void Start()
        {
            StartCoroutine(LateStart(0.1f));        
        }
        
        IEnumerator LateStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            _rend = GetComponentInChildren<Renderer>();
            _baseTransparency = _rend.material.GetFloat("_Alpha");
        }
        
        void Update()
        {
            if(_rend != null)
            {
                var transparency = _baseTransparency + _baseTransparency * Mathf.Sin(Time.time*_speed)*_range;
                _rend.material.SetFloat("_Alpha", transparency);           
            }
        }
    }
}