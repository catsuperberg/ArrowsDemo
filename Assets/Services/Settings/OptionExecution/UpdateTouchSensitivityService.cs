using System;
using System.Linq;
using UnityEngine;
using GamePlay;

namespace GameSettings
{
    public class UpdateTouchSensitivityService : MonoBehaviour, ISettingsCommandService
    {
        public void Execute(object command)
        {
            var cmd = (UpdateTouchSensitivity)command;
            var Sensitivity = cmd.Sensitivity;
            var touchComponents = FindObjectsOfType<MonoBehaviour>().OfType<ISensitivitySetable>(); // HACK should probably figure out how to do this with DI
            Debug.Log("trying to change sensitivity");
            foreach(var component in touchComponents)
            {
                component.UpdateSensitivity(Sensitivity);
            }
        }
    }
    
    public class UpdateTouchSensitivity
    {
        public float Sensitivity {get; set;}
    }
}