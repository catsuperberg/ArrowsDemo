using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetScripts.Visual
{
    public class PulsatingGlow : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 0.5f;
        [SerializeField]
        private float _range = 0.12f;
        
        private Renderer _rend;
        private float _baseGlowVisibility;
        
        void Start()
        {
            StartCoroutine(LateStart(0.1f));        
        }
        
        IEnumerator LateStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            _rend = GetComponentInChildren<Renderer>();
            _baseGlowVisibility = _rend.material.GetFloat("_SecondaryTextureAlpha");
        }
        
        void Update()
        {
            if(_rend != null)
            {
                var intensity = _baseGlowVisibility + _range + Mathf.Sin(Time.time*_speed)*_range;  
                _rend.material.SetFloat("_SecondaryTextureAlpha", intensity);       
            }
        }
    }
}
