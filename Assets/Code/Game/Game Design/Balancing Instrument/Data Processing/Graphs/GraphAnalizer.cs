using System;
using UnityEngine;

namespace Game.GameDesign
{        
    public abstract class GraphAnalizer
    {
        protected Texture2D _cachedTexture;      
        protected Vector2Int _cachedDimensions;    
        
        /// <summary> Only works on main thread </summary>
        protected Texture2D GraphTexture(Vector2Int dimensions, Func<string> generateImage)
        {
            if(_cachedTexture != null && dimensions == _cachedDimensions)
                return _cachedTexture;
            
            var texture = new Texture2D(1,1, TextureFormat.RGBA32, false, false);
            var base64Image = generateImage(); 
            texture.LoadImage(Convert.FromBase64String(base64Image));
            
            _cachedTexture = texture;
            _cachedDimensions = dimensions;
            return _cachedTexture;
        }
    }
}