using System;

namespace Settings
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
    }  
}
