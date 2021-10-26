namespace GameSettings
{
    public interface ISettingsService
    {
        public void SetOption<T>(AudioVisualOptions optionToSet, T value);
        public void ApplySettings();
        public void SaveSettings();
        public void LoadSettings();
    }
}
