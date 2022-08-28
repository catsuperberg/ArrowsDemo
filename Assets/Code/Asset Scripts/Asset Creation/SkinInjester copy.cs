using Game.Gameplay.Meta.Skins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public interface ISkinInjester
    {        
        public void InjestSkins();
    }
}