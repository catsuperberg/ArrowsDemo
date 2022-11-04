using GameMath;
using System;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class GameBalanceWindow : EditorWindow
{
    string _buttonCallResult = "[RESULT]";
    
    [MenuItem("Window/Game/Game Balance")]
    public static void OpenWindow()
    {
        EditorWindow.GetWindow<GameBalanceWindow>("Game Balance");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Reward and pricing balance", EditorStyles.centeredGreyMiniLabel);
        
        GUI.enabled = false;
        EditorGUILayout.TextField("Call Result", _buttonCallResult);
        GUI.enabled = true;
        
        if(GUILayout.Button("Generate Result"))
            FunctionCall();
    }
    
    void FunctionCall()
    {
        // _buttonCallResult = "Function called: " + GlobalRandom.RandomString(5);
        _buttonCallResult = RunScriptAsProcess();
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
