using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameSettings
{
    public class OptionsExecutor
    {
        private SettingsData _currentSettings = new SettingsData();
        
        public void ApplyChanged(SettingsData settings)
        {
            var currentSettings = _currentSettings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            var newSettings = settings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            for(int i = 0; i < currentSettings.Count(); i++)
            {
                var oldValue = currentSettings[i].GetValue(_currentSettings);
                var newValue = newSettings[i].GetValue(settings);
                if(oldValue != newValue)
                {
                    if(currentSettings[i].Name == "ResolutionScaling")
                    {
                        Debug.LogWarning("Proceding to change resolution scaling");
                        
                        var scaler = Camera.main.GetComponent<ResolutionScaler>();
                        if(scaler != null)
                            scaler.SetScale(Convert.ToSingle(newValue));
                    }
                }
            }
        }
    }
}