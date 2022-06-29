using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Meta.Skins
{
    public interface ISkinProvider
    {
        IList<string> Names {get;}
        
        UnityEngine.Object LoadResource(string name);
        Sprite Icon(string name);        
    }
}