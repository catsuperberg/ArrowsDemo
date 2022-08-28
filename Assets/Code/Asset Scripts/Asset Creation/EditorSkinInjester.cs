using Game.Gameplay.Meta.Skins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR

namespace AssetScripts.AssetCreation
{
    public abstract class EditorSkinInjester : MonoBehaviour
    {           
        // ISkinInjester _injester;
        
        // public IEditorSkinInjester(ISkinInjester injester)
        // {
        //     _injester = injester ?? throw new ArgumentNullException(nameof(injester));
        // }
        
        virtual public void InjestSkins() => throw new NotImplementedException();
            // => _injester.InjestSkins();
    }
}

#endif