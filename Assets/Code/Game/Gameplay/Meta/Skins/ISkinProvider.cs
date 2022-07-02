using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Game.Gameplay.Meta.Skins
{
    public interface ISkinProvider
    {
        IList<string> Names {get;}
        
        UnityEngine.Object LoadResource(string name);
        Sprite Icon(string name);        
        BigInteger Price(string name);        
    }
}