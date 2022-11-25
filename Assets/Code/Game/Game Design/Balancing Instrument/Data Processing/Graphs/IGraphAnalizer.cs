using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.GameDesign
{        
    public interface IGraphAnalizer
    {
        public GraphType Type {get;}
        public Texture2D GraphTexture(Vector2Int textureSize);
    }
}