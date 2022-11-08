using ExtensionMethods;
using GameDesign;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Zenject;

using Debug = UnityEngine.Debug;

public class SimProgressReport
{
    public int SimulationsDone => _simulationsDone;
    int _simulationsDone;
    public readonly int FinalSimultationCount;

    public void IncrementDone()
    {
        System.Threading.Interlocked.Increment(ref _simulationsDone);
    }
    
    public SimProgressReport(int finalSimultationCount)
    {
        FinalSimultationCount = finalSimultationCount;
    }
    
    public float ProgressPart()
        => (float)SimulationsDone/(float)FinalSimultationCount;
}

public class ZenjectBalanceWindow : ZenjectEditorWindow
{
    string _buttonCallResult = "[RESULT]";
    string _simulatorCallResult = "[NONE]";
    DataRetriever _dataRetriever;
    DataPlotter _dataPlotter;
    int _progressId;
    
    Texture2D _firstGraph;
    
    public override void InstallBindings()
    {
        InstrumentInstaller.Compose(Container);
        _dataRetriever = Container.Resolve<DataRetriever>();
        _dataPlotter = new DataPlotter();
    }
    
    [MenuItem("Window/Game/Game Balance (zenject)")]
    public static ZenjectBalanceWindow GetOrCreateWindow()
        => EditorWindow.GetWindow<ZenjectBalanceWindow>("Game Balance (zenject)");
    
    override public void OnGUI()
    {   
        if(GUILayout.Button("Simulate"))
            _ = System.Threading.Tasks.Task.Run(CallSimulation);
        
        EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));        
        EditorGUILayout.LabelField("Result Plot");  
        var rect = EditorGUILayout.GetControlRect(hasLabel: true, height:350);
        EditorGUI.DrawPreviewTexture(rect, _firstGraph?? EditorGUIUtility.whiteTexture);  
        EditorGUILayout.EndVertical();                
    }
        
    async void CallSimulation()
    {
        var uniquePlayerCount = 50;
        _progressId = Progress.Start("Simulation progress");        
        var progressIndicator = new Progress<SimProgressReport>(OnSimultaionProgressReport);        
        var results = await _dataRetriever.SimulateForStatistics(uniquePlayerCount, progressIndicator);       
        Progress.Remove(_progressId);
        DrawGraph(results);
    }
    
    void DrawGraph(IEnumerable<Game.GameDesign.PlaythroughData> results)
    {    
        var maxRuns = results
            .Select(result => result.Runs.Count())
            .Max();
        var range = Enumerable.Range(0, maxRuns);
        var averageRewards = range
            .Select(runNumber => (double)BigIntCalculations.Mean(results.Select(result => result.Runs.ElementAtOrDefault(runNumber))
                .Where(run => run != null)
                .Select(run => run.FinalScore)));     
        
        IEnumerable<ChartDataPoints> dataPoints = range.Select(run => new ChartDataPoints(run, averageRewards.ElementAt(run)));          
        var textureB64 = _dataPlotter.PlotXYBase64(dataPoints, new Vector2Int(650, 320));        
        this.StartCoroutine(DrawGraphTexture(textureB64));  
    }
    
    IEnumerator DrawGraphTexture(string base64Texture)
    {        
        var texture = new Texture2D(1,1);
        texture.LoadImage(Convert.FromBase64String(base64Texture));   
        _firstGraph = texture;
        yield return null;
    }
    
    
    
    void PythonButtonCall()
    {
        _buttonCallResult = RunScriptAsProcess();
    }
    
    void OnSimultaionProgressReport(SimProgressReport progress)
    {
        if(Progress.GetProgressById(_progressId) == null)
            return;
        Progress.Report(_progressId, progress.ProgressPart(), description: $"Count: {progress.SimulationsDone} / {progress.FinalSimultationCount}");
    }
    
    string RunScriptAsProcess()
    {
        string result = "";
        try
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"python";
            start.Arguments = string.Format("{0} {1}", "\"C:\\Unity\\ArrowsDemo\\Assets\\Code\\Game\\Game Design\\Reward And Price\\Test.py\"", "");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;
            var process = Process.Start(start);
            
            StreamReader reader = process.StandardOutput;
            result = reader.ReadToEnd();

            process.WaitForExit();
        }
        catch (Exception ex)
        {
            result = "Couldn't run python script: " + ex.Message;
            Debug.LogWarning("Couldn't run python script: " + ex.Message);
        }
        return result;
    }
}