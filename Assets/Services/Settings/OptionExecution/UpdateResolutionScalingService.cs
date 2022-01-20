using System;
using UnityEngine;

namespace GameSettings
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