using System.Collections.Generic;

namespace GameSettings
{
    public interface ISettingsExecutorService
    {
        public void UpdateSettings(SettingsContainer settings, IEnumerable<Settings> settingsToChange);
    }
}