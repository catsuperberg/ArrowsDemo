using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameSettings
{
    public class ChangedSettingsExecutorService : ISettingsExecutorService
    {
        // private SettingsData _currentSettings = new SettingsData();
        // private HashSet<Settings> _settingsToChange = new HashSet<Settings>();
        
        public void UpdateSettings(SettingsContainer settings, IEnumerable<Settings> settingsToChange)
        {            
            foreach(var setting in settingsToChange)
            {
                var command = settings.CreateCommandForSetting(setting);
                var service = settings.CreateServiceForSetting(setting);
                if(service != null)
                    service.Execute(command);
                else
                    Debug.Log("No service provided to settings executor. Setting is: " + setting);
            }
            
            // var currentSettings = _currentSettings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            // var newSettings = settings.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            // for(int i = 0; i < currentSettings.Count(); i++)
            // {
            //     Debug.Log("option name: " + currentSettings[i].Name);
            //     var option = new Settings().OptionNameToEnum(currentSettings[i].Name);
            //     var oldValue = currentSettings[i].GetValue(_currentSettings);
            //     var newValue = newSettings[i].GetValue(settings);
            //     Debug.Log("oldValue: " + oldValue + " | " + "newValue: " + newValue);
            //     if(oldValue != newValue)
            //         UpdateSetting(option, newValue);
            // }
            // _currentSettings = settings;
        }
        
        // void UpdateSetting(Settings option, object value)
        // {    
        //     var command = option.CreateCommand(value);
        //     var service = option.CreateService();
        //     if(service != null)
        //         service.Execute(command);
        // } 
        
        // object CreateCommand(AudioVisualOptions option, object value)
        // {
        //     switch(option)
        //     {
        //         case AudioVisualOptions.ResolutionScaling:
        //             var command = new UpdateResolutoionScaling()
        //                 {ScaleFactor = (float)Convert.ChangeType(value, option.OptionType())};
        //             return command;
        //         default:
        //             break;
        //     }
        //     return null;
        // }
        
        // ISettingsCommandService GetServiceForCommand(object command)
        // {
        //     if(command == null)
        //         return null;                
        //     if(command.GetType() == typeof(UpdateResolutoionScaling))
        //         return new UpdateResolutionScalingService();
        //     else
        //         return null;
        // }
    }
}