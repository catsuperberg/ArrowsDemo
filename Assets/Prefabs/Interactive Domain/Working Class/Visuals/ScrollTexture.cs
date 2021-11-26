using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    [SerializeField]
    private float _speed = 0.5f;
    
    Renderer _rend;
    
    void Start()
    {
        _rend = GetComponentInChildren<Renderer>();
    }
    
    void Update()
    {
        var offset = Time.time * _speed;
        _rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
