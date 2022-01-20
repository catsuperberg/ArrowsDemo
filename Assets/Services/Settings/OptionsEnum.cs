using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameSettings
{
    public enum Settings
    {
        MasterVolume,
        MusicVolume,
        SfxVolume,
        Vibration,
        TouchSensitivity,
        ResolutionScaling,
        Graphics,
    }    
    
    public enum GraphicsPresets
    {
        Low = 0,
        Medium = 1,
        Heigh = 2
    }
    
    public static class OptionsHelper
    {
        public static Settings OptionNameToEnum(this Settings option, string name)
        {    
            Settings result;        
            if(Enum.TryParse(name, out result))
            {
                return result;
            }
            else
                throw new System.Exception("no option with the name: " + name);
        }
        
        // public static ISettingsCommandService CreateService(this Settings option)
        // {
        //     Debug.Log("option from CreateService() " + option);
        //     switch(option)
        //     {                
        //         case Settings.MasterVolume:
        //             return null;
        //         case Settings.MusicVolume:
        //             return null;
        //         case Settings.SfxVolume:
        //             return null;
        //         case Settings.Vibration:
        //             return null;
        //         case Settings.ResolutionScaling:
        //             return new UpdateResolutionScalingService();
        //         case Settings.Graphics:
        //             return null;
        //         default:
        //             throw new System.Exception("No valid option for ISettingsCommandService");
        //     }
        // }
        
        // public static object CreateCommand(this Settings option, object value)
        // {
        //     Debug.Log("option from CreateCommand() " + option);
        //     switch(option)
        //     {
                
        //         case Settings.MasterVolume:
        //             return null;
        //         case Settings.MusicVolume:
        //             return null;
        //         case Settings.SfxVolume:
        //             return null;
        //         case Settings.Vibration:
        //             return null;
        //         case Settings.ResolutionScaling:
        //             var command = new UpdateResolutoionScaling()
        //                 {ScaleFactor = Convert.ToSingle(value)};
        //             return command;
        //         case Settings.Graphics:
        //             return null;
        //         default:
        //             throw new System.Exception("No valid option for CreateCommand");
        //     }
        // }
        
        // public static System.Type OptionType(Settings option)
        // {
        //     switch (option)
        //     {
        //         case Settings.MasterVolume:
        //             return typeof(float);
        //         case Settings.MusicVolume:
        //             return typeof(float);
        //         case Settings.SfxVolume:
        //             return typeof(float);
        //         case Settings.Vibration:
        //             return typeof(bool);
        //         case Settings.ResolutionScaling:
        //             return typeof(float);
        //         case Settings.Graphics:
        //             return typeof(int);
        //         default:
        //             return typeof(int);
        //     }
        // }
    }  
}
