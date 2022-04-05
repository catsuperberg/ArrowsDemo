using GameMath;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameState
{    
    public class AdPlayer : MonoBehaviour
    {
        [SerializeField]
        List<Texture> AdPictures;
        [SerializeField]
        RawImage RawImageComponent;
        [SerializeField]
        AspectRatioFitter AspectRatioComponent;
        
        void Awake()
        {
            var texture = AdPictures[GlobalRandom.RandomInt(0, AdPictures.Count)];
            var aspectRatio = (float)texture.width/(float)texture.height;
            RawImageComponent.texture = texture;
            AspectRatioComponent.aspectRatio = aspectRatio;
        }
    }
}