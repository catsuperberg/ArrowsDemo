using System.Collections.Generic;

namespace Settings
{
    public interface ISettingsExecutorService
    {
        public void UpdateSettings(SettingsContainer settings, IEnumerable<Settings> settingsToChange);
    }
}