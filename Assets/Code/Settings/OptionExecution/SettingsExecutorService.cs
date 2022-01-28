using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Settings
{
    public class ChangedSettingsExecutorService : ISettingsExecutorService
    {        
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
        }
    }
}