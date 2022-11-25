using UnityEngine;

namespace Game.GameDesign
{        
    public interface IGraphAnalizer
    {
        public GraphType Type {get;}
        public Texture2D GetTexture(Vector2Int textureSize);
    }
}