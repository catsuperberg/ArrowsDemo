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
    int _progressId;
    
    public override void InstallBindings()
    {
        InstrumentInstaller.Compose(Container);
        _dataRetriever = Container.Resolve<DataRetriever>();
    }
    
    [MenuItem("Window/Game/Game Balance (zenject)")]
    public static ZenjectBalanceWindow GetOrCreateWindow()
        => EditorWindow.GetWindow<ZenjectBalanceWindow>("Game Balance (zenject)");
    
    
    override public void OnGUI()
    {
        GUILayout.Label("Reward and pricing balance", EditorStyles.centeredGreyMiniLabel);
        
        GUI.enabled = false;
        EditorGUILayout.TextField("Call Result", _buttonCallResult);
        GUI.enabled = true;
        
        if(GUILayout.Button("Generate Result"))
            PythonButtonCall();
        
        
        
        GUI.enabled = false;
        EditorGUILayout.TextField("Answer from simultions", _simulatorCallResult);
        GUI.enabled = true;
        
        if(GUILayout.Button("Call simulation"))
            _ = System.Threading.Tasks.Task.Run(CallSimulation);
    }
    
    void PythonButtonCall()
    {
        _buttonCallResult = RunScriptAsProcess();
    }
    
    async void CallSimulation()
    {
        var uniquePlayerCount = 25;
        _progressId = Progress.Start("Simulation progress");        
        var progressIndicator = new Progress<SimProgressReport>(OnSimultaionProgressReport);        
        var results = await _dataRetriever.SimulateForStatistics(uniquePlayerCount, progressIndicator);        
        _simulatorCallResult = results.First().PlayerHeader;
        Progress.Remove(_progressId);
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