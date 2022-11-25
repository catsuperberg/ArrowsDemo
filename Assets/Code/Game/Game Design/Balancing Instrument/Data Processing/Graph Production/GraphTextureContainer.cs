using System;
using UnityEngine;

namespace Game.GameDesign
{
    public class GraphTextureContainer
    {
        public readonly Vector2Int Dimensions;
        public readonly string Base64Image;
        public Texture2D Texture {get; private set;}
        public bool Unrendered => Texture == null;


        public GraphTextureContainer(string base64Image, Vector2Int dimensions, Texture2D texture = null)
        {
            Base64Image = base64Image;
            Dimensions = dimensions;
            Texture = texture;
        }
                
        /// <summary> Only works on main thread </summary>
        public void RenderTexture()
        {
            Texture = new Texture2D(1,1, TextureFormat.RGBA32, false, false);
            Texture.LoadImage(Convert.FromBase64String(Base64Image));
        }
    }
}