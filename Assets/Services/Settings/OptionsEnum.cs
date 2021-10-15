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
    
    
    public static class optionsHelper
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
