using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace AssetScripts.AssetCreation
{
    [CustomEditor (typeof (EditorSkinInjester), true)]
    public class EditorSkinInjesterInspector : Editor
    {
        public override void OnInspectorGUI()
        {        
            DrawDefaultInspector();
                    
            EditorSkinInjester injester = (EditorSkinInjester)target;
            
            if(GUILayout.Button("Injest from Asset Injest folder"))
            {
                injester.InjestSkins();
            }
        }
    }
}
#endif
