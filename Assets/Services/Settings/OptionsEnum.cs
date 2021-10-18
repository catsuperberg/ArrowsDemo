namespace GameSettings
{
    public enum AudioVisualOptions
    {
        MasterVolume,
        MusicVolume,
        SfxVolume,
        Vibration,
        ResolutionScaling,
        Graphics
    }    
    
    public enum GraphicsPresets
    {
        Low = 0,
        Medium = 1,
        Heigh = 2
    }
    
    public static class OptionsHelper
    {
        public static System.Type OptionType(AudioVisualOptions option)
        {
            switch (option)
            {
                case AudioVisualOptions.MasterVolume:
                    return typeof(float);
                case AudioVisualOptions.MusicVolume:
                    return typeof(float);
                case AudioVisualOptions.SfxVolume:
                    return typeof(float);
                case AudioVisualOptions.Vibration:
                    return typeof(bool);
                case AudioVisualOptions.ResolutionScaling:
                    return typeof(float);
                case AudioVisualOptions.Graphics:
                    return typeof(int);
                default:
                    return typeof(int);
            }
        }
    }
}
