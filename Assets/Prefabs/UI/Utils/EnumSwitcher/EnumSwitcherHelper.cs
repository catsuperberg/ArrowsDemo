using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

namespace UiUtils
{
    public class EnumSwitcherHelper : MonoBehaviour
    {        
        [SerializeReference]
        public IEnumSelectable SelectableEnumImplementation;
        
        [SerializeField]
        private GameObject CurrentStateOverlay;
        
        [SerializeField]
        public int CurrentPreset;
        
        public void SwitchToNextOption()
        {
            if((int)CurrentPreset < SelectableEnumImplementation.Length()-1)
                CurrentPreset++;
            else
                CurrentPreset = 0;
            UpdateCurrentOverlay();            
        }
        
        public void UpdateCurrentOverlay()
        {
            var textField = CurrentStateOverlay.GetComponent<TMP_Text>();
            textField.text = SelectableEnumImplementation.ValueName(CurrentPreset);
        }
        
    }
}


// TODO: Rework so there's no need to instantiate Implementation every time
namespace UiUtils
{
    [CustomEditor(typeof(EnumSwitcherHelper))]
    class EnumSwitcherHelperInspector : Editor
    {
        private Type[] _implementations;
        private int _implementationTypeIndex;

        public override void OnInspectorGUI()
        {
            EnumSwitcherHelper customBehaviour = target as EnumSwitcherHelper;
            //specify type
            if (customBehaviour == null)
            {
                return;
            }
            
            if (_implementations == null || GUILayout.Button("Refresh implementations"))
            {
                //this is probably the most imporant part:
                //find all implementations of IEnumSelectable using System.Reflection.Module
                _implementations = GetImplementations<IEnumSelectable>().Where(impl=>!impl.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();
            }

            EditorGUILayout.LabelField($"Found {_implementations.Count()} implementations");            
            
            //select implementation from editor popup
            _implementationTypeIndex = EditorGUILayout.Popup(new GUIContent("Implementation"),
                _implementationTypeIndex, _implementations.Select(impl => impl.FullName).ToArray());

            if (GUILayout.Button("Create instance"))
            {
                //set new value
                customBehaviour.SelectableEnumImplementation = (IEnumSelectable) Activator.CreateInstance(_implementations[_implementationTypeIndex]);
            }

            base.OnInspectorGUI();
        }
        
        private static Type[] GetImplementations<T>()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());

            var interfaceType = typeof(T);
            return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
        }
    }
}