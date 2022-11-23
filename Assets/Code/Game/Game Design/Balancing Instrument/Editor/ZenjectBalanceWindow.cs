using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game.GameDesign
{
    public class ZenjectBalanceWindow : ZenjectEditorWindow
    {                
        const int _graphHeight = 280; 
        const int _defaultPlaythroughsToRun = 800; 
        const int _defaultSimulatorRepeats = 4; 
        int _uniquePlaythroughsToRun = _defaultPlaythroughsToRun; 
        int _uniqueSimulatorRepeats = _defaultSimulatorRepeats; 
        
        BalanceController _balanceController;      
        Rect _graphRect;    
        
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
            _uniquePlaythroughsToRun = EditorGUILayout.IntSlider(
                "Playthroughs To Simulate", _uniquePlaythroughsToRun, 25, 5000, GUILayout.MaxWidth(380));
            _uniqueSimulatorRepeats = EditorGUILayout.IntSlider(
                "Simulator Repeats", _uniqueSimulatorRepeats, 1, 20, GUILayout.MaxWidth(380));
            
            
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
            if(Event.current.type == EventType.Repaint)   
                _graphRect = EditorGUILayout.GetControlRect(hasLabel: true, height:_graphHeight); 
            else
                EditorGUILayout.GetControlRect(hasLabel: true, height:_graphHeight);
            EditorGUI.DrawPreviewTexture(_graphRect, _balanceController.DrawGraph(type, _graphRect), null, scaleMode: ScaleMode.ScaleToFit); 
        }
            
        async void CallSimulation()
        {
            _balanceController.SimulatePlaythroughs(_uniquePlaythroughsToRun, _uniqueSimulatorRepeats);
        }     
    }
}