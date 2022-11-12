using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Game.GameDesign
{    
    public class ChartFormulaGetter 
    {
        
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
}