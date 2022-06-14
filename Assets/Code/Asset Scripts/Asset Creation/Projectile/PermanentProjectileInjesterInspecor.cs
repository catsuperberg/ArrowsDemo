using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    [CustomEditor(typeof(PermanentProjectileInjester))]
    public class trackScriptInspector : Editor
    {
        public override void OnInspectorGUI()
        {        
            DrawDefaultInspector();
                    
            PermanentProjectileInjester injester = (PermanentProjectileInjester)target;
            
            if(GUILayout.Button("Injest from Asset Injest folder"))
            {
                injester.InjestProjectiles();
            }
        }
    }
}
