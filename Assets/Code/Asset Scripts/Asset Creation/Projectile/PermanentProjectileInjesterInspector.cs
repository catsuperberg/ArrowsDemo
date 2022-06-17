using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace AssetScripts.AssetCreation
{
    [CustomEditor(typeof(PermanentProjectileInjester))]
    public class PermanentProjectileInjesterInspector : Editor
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
#endif
