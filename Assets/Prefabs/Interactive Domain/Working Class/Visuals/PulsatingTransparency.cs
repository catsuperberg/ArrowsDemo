using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _rend = GetComponentInChildren<Renderer>();
        _baseTransparency = _rend.material.color.a;
    }
    
    void Update()
    {
        var transparency = _baseTransparency + _baseTransparency * Mathf.Sin(Time.time*_speed)*_range;
        var color = _rend.material.color;
        color.a = transparency;
        _rend.material.color = color;
    }
}
