using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game.GameDesign
{
    public class ZenjectBalanceWindow : ZenjectEditorWindow
    {                
        const int _graphHeight = 280; 
        const int _defaultPlayersToSimulate = 350; 
        int _uniquePlayersToSimulate = _defaultPlayersToSimulate; 
        
        BalanceController _balanceController;
        
        Rect _graphRect = new Rect();    
        
        
        public override void InstallBindings()
        {
            InstrumentInstaller.Compose(Container);
            _balanceController = Container.Resolve<BalanceController>();
        }
                
        [MenuItem("Window/Game/Game Balance (zenject)")]
        public static ZenjectBalanceWindow GetOrCreateWindow()
            => EditorWindow.GetWindow<ZenjectBalanceWindow>("Game Balance (zenject)");
        
        override public void OnGUI()
        {   
            _uniquePlayersToSimulate = EditorGUILayout.IntSlider(
                "Players To Simulate", _uniquePlayersToSimulate, 25, 5000, GUILayout.MaxWidth(380));
            
            
            if(GUILayout.Button("Simulate"))
                _ = System.Threading.Tasks.Task.Run(CallSimulation);
            
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));      
            RenderGraph(GraphType.RewardPerRun);   
            RenderGraph(GraphType.UpgradesPerRun);   
            EditorGUILayout.EndVertical();                
        }
        
        void RenderGraph(GraphType type)
        {            
            EditorGUILayout.LabelField(
                type.Label(), new GUIStyle(GUI.skin.label) 
                    {alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Bold}, 
                GUILayout.Height(EditorGUIUtility.singleLineHeight*1.5f));  
            _graphRect = EditorGUILayout.GetControlRect(hasLabel: true, height:_graphHeight);
            EditorGUI.DrawPreviewTexture(_graphRect, _balanceController.DrawGraph(type, _graphRect), null, scaleMode: ScaleMode.ScaleToFit); 
        }
            
        async void CallSimulation()
        {
            _balanceController.SimulatePlaythroughs(_uniquePlayersToSimulate);
        }     
    }
}