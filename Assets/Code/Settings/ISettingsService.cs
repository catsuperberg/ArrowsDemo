using System;

namespace Settings
{
    public interface ISettingsService
    {
        public void SetOption<T>(Settings optionToSet, T value) where T : IComparable;
        public void ApplySettings();
        public void SaveSettings();
        public void LoadSettings();
    }
}
