using ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Zenject;

using Vector2 = UnityEngine.Vector2;

namespace Game.GameDesign
{    
    public class ZenjectBalanceWindow : ZenjectEditorWindow
    {                
        Vector2 _scrollPos;
        const int _graphHeight = 280; 
        const int _defaultPlaythroughsToRun = 800; 
        const int _defaultSimulatorRepeats = 4; 
        int _uniquePlaythroughsToRun = _defaultPlaythroughsToRun; 
        int _uniqueSimulatorRepeats = _defaultSimulatorRepeats; 
        CompletionConditions.Controlls _completionConditions = 
            new CompletionConditions.Controlls(40, 3, BigIntegerHelper.ParseFromScientific("1.0e20"));
        
        BalanceController _balanceController;      
        Rect _graphRect;    
        
        public override void InstallBindings()
        {
            InstrumentInstaller.Compose(Container);
            _balanceController = Container.Resolve<BalanceController>();
        }
                
        [MenuItem("Window/Game Design/Game Balance")]
        public static ZenjectBalanceWindow GetOrCreateWindow()
            => EditorWindow.GetWindow<ZenjectBalanceWindow>("Game Balance");
        
        override public void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false);

            DefineView();

            EditorGUILayout.EndScrollView();

        }

        private void DefineView()
        {                      
            DefineSimulation();
            DefineAveragedSimulation();
        }
        
        private void DefineSimulation()
        {            
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Label(
                "Simulation", new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Bold });

            DefineSimulationParameters();

            if (GUILayout.Button("Simulate"))
                _ = System.Threading.Tasks.Task.Run(CallSimulation);

            DefineSimulationResults();  
            EditorGUILayout.EndVertical();
        }

        private void DefineSimulationResults()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            RenderGraph(GraphType.RewardPerRun);
            RenderGraph(GraphType.UpgradesPerRun);
            RenderGraph(GraphType.UpgradesPerReward);
            RenderGraph(GraphType.TimeToReward);
            DisplayValue(SimValueType.PlaythroughTime);
            DisplayValue(SimValueType.GateSelectorStats);
            DisplayValue(SimValueType.AdSelectorStats);
            EditorGUILayout.EndVertical();
        }

        private void DefineSimulationParameters()
        {
            DefineCompletionConditions();

            _uniquePlaythroughsToRun = EditorGUILayout.IntSlider(
                "Playthroughs To Simulate", _uniquePlaythroughsToRun, 25, 5000, GUILayout.MaxWidth(380));
            _uniqueSimulatorRepeats = EditorGUILayout.IntSlider(
                "Simulator Repeats", _uniqueSimulatorRepeats, 1, 20, GUILayout.MaxWidth(380));
        }

        private void DefineCompletionConditions()
        {
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Label(
                "Completion conditions: ", new GUIStyle(GUI.skin.label)
                {alignment = TextAnchor.MiddleLeft, fontSize = 16, fontStyle = FontStyle.Bold});
            _completionConditions.PlayMinutes = EditorGUILayout.Slider(
                "Playthrough minutes", _completionConditions.PlayMinutes, 5, 120, GUILayout.MaxWidth(380));
            _completionConditions.LongestRunMinutes = EditorGUILayout.Slider(
                "Single run minutes", _completionConditions.LongestRunMinutes, 1, 15, GUILayout.MaxWidth(380));
            _completionConditions.MaxReward = BigIntegerHelper.ParseFromScientific(EditorGUILayout.TextField(
                "Max Reward", _completionConditions.MaxReward.ToString("#.0e0")));
            EditorGUILayout.EndVertical();
        }
        
        private void DefineAveragedSimulation()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            GUILayout.Label(
                "Average player simulation", new GUIStyle(GUI.skin.label)
                { alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Bold });
            
            if (GUILayout.Button("Simulate"))
                _ = System.Threading.Tasks.Task.Run(CallAverageSimulation);
            
            DefineAverageResults();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DefineAverageResults()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));            
            DisplayValue(SimValueType.AveragePlaythroughTime);
            RenderGraph(GraphType.AverageRewardPerRun);
            RenderGraph(GraphType.AverageUpgradesPerRun);
            RenderGraph(GraphType.AverageUpgradesPerReward);
            RenderGraph(GraphType.AverageTimeToReward);
            EditorGUILayout.EndVertical();
        }
        

        void RenderGraph(GraphType type)
        {            
            EditorGUILayout.LabelField(
                type.Label(), new GUIStyle(GUI.skin.label) 
                    {alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Normal}, 
                GUILayout.Height(EditorGUIUtility.singleLineHeight*1.5f));  
            if(Event.current.type == EventType.Repaint)   
                _graphRect = EditorGUILayout.GetControlRect(hasLabel: true, height:_graphHeight); 
            else
                EditorGUILayout.GetControlRect(hasLabel: true, height:_graphHeight);
            EditorGUI.DrawPreviewTexture(_graphRect, _balanceController.DrawGraph(type, _graphRect), null, scaleMode: ScaleMode.ScaleToFit); 
        }
        
        void DisplayValue(SimValueType type)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));  
            GUILayout.Label(
                type.Label(), new GUIStyle(GUI.skin.label) 
                    {alignment = TextAnchor.MiddleLeft, fontSize = 16, fontStyle = FontStyle.Normal});
            GUILayout.Label(
                _balanceController.GetValue(type), new GUIStyle(GUI.skin.textField) 
                    {alignment = TextAnchor.MiddleLeft, fontSize = 16}, GUILayout.MaxWidth(400));
            EditorGUILayout.EndHorizontal();     
        }
        
        void DisplayBigValue(SimValueType type)
        {
            GUILayout.Label(
                type.Label(), new GUIStyle(GUI.skin.label) 
                    {alignment = TextAnchor.MiddleLeft, fontSize = 16, fontStyle = FontStyle.Normal});
            GUILayout.Label(
                _balanceController.GetValue(type), new GUIStyle(GUI.skin.textArea) 
                    {alignment = TextAnchor.MiddleLeft, fontSize = 16});
        }
            
        async void CallSimulation()
        {
            _balanceController.SimulatePlaythroughs(_uniquePlaythroughsToRun, _uniqueSimulatorRepeats, _completionConditions.ToConditions());
        }     
        
        async void CallAverageSimulation()
        {
            _balanceController.SimulateAveragePlayer(30, _completionConditions.ToConditions());            
        }
    }
}