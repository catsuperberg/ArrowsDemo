using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSettings
{
    public class SettingsContainer
    {
        public SettingsData Data {get; private set;}
        // public List<Settings> SettingsToChange {get; private set;} = new List<Settings>();
        
        public SettingsContainer(SettingsData data)
        {
            if(data == null)
                throw new System.Exception("SettingsData not provided for SettingsContainer");
                
            Data = data;
        }
        
        public bool UpdateSetting<T>(Settings setting, T value) where T : IComparable
        {
            bool changed = false;
            switch (setting)
            {
                case Settings.MasterVolume:                
                    changed = UpdateValueIfDifferent(ref Data.MasterVolume, value);
                    break;
                case Settings.MusicVolume:
                    changed = UpdateValueIfDifferent(ref Data.MusicVolume, value);
                    break;
                case Settings.SfxVolume:
                    changed = UpdateValueIfDifferent(ref Data.SfxVolume, value);
                    break;
                case Settings.Vibration:
                    changed = UpdateValueIfDifferent(ref Data.Vibration, value);
                    break;
                case Settings.TouchSensitivity:
                    changed = UpdateValueIfDifferent(ref Data.TouchSensitivity, value);
                    break;
                case Settings.ResolutionScaling:
                    changed = UpdateValueIfDifferent(ref Data.ResolutionScaling, value);
                    break;
                case Settings.Graphics:
                    changed = UpdateValueIfDifferent(ref Data.Graphics, value);
                    break;
                default:
                    throw new System.Exception("No such setting as: " + Enum.GetName(typeof(Settings), setting));
            }
            return changed;
            // if(changed)
            //     SettingsToChange.Add(setting);
        }    
        
        public object CreateCommandForSetting(Settings setting)
        {
            object command;
            switch(setting)
            {                
                case Settings.MasterVolume:
                    return null;
                case Settings.MusicVolume:
                    return null;
                case Settings.SfxVolume:
                    return null;
                case Settings.Vibration:
                    return null;
                case Settings.TouchSensitivity:
                    command = new UpdateTouchSensitivity()
                        {Sensitivity = Data.TouchSensitivity};
                    return command;
                case Settings.ResolutionScaling:
                    command = new UpdateResolutoionScaling()
                        {ScaleFactor = Data.ResolutionScaling};
                    return command;
                case Settings.Graphics:
                    return null;
                default:
                    throw new System.Exception("No valid setting for CreateCommand()");
            }
        }
        
        public ISettingsCommandService CreateServiceForSetting(Settings setting)
        {
            switch(setting)
            {                
                case Settings.MasterVolume:
                    return null;
                case Settings.MusicVolume:
                    return null;
                case Settings.SfxVolume:
                    return null;
                case Settings.Vibration:
                    return null;
                case Settings.TouchSensitivity:
                    return new UpdateTouchSensitivityService();
                case Settings.ResolutionScaling:
                    return new UpdateResolutionScalingService();
                case Settings.Graphics:
                    return null;
                default:
                    throw new System.Exception("No valid setting for CreateServiceForSetting()");
            }
        }    
        
        bool UpdateValueIfDifferent<G, T>(ref G fieldToChange, T value) where G : IComparable where T : IComparable
        {
            if(typeof(G) != typeof(T))
                throw new System.Exception("Value provided for the option is not of the right Type, Types are: " + typeof(G) + " and " + typeof(T));
            if(fieldToChange.CompareTo(value) != 0)
            {
                fieldToChange = (G)Convert.ChangeType(value, typeof(G));
                return true;
            }
            else
                return false;                
        }
    }    
    
    public class SettingsData
    {
        public float MasterVolume = float.NaN;
        public float MusicVolume = float.NaN;
        public float SfxVolume = float.NaN;
        public bool Vibration = false;
        public float TouchSensitivity = float.NaN;
        public float ResolutionScaling = float.NaN;
        public int Graphics = 0;
    }
    
}
