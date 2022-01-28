using Game.Gameplay.Runtime.RunScene.GameCamera;
using System;
using UnityEngine; 

namespace Settings
{
    public class UpdateResolutionScalingService : ISettingsCommandService
    {
        public void Execute(object command)
        {
            var cmd = (UpdateResolutoionScaling)command;
            var scaleFactor = cmd.ScaleFactor;
            var scaler = Camera.main.GetComponent<ResolutionScaler>();
            if(scaler != null)
                scaler.SetScale(Convert.ToSingle(scaleFactor));
        }
    }
    
    public class UpdateResolutoionScaling
    {
        public float ScaleFactor {get; set;}
    }
}