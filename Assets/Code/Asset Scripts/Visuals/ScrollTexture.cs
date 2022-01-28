using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetScripts.Visual
{
    public class ScrollTexture : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 0.5f;
        
        private Renderer _rend;
        
        void Start()
        {
            StartCoroutine(LateStart(0.1f));        
        }
        
        IEnumerator LateStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            _rend = GetComponentInChildren<Renderer>();
        }
        
        void Update()
        {
            if(_rend != null)
            {
                var offset = Time.time * _speed;
                _rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
            }
        }
    }
}