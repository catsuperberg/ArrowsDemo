namespace GameStorage
{
    public interface IGameFolders
    {
        public string StreamingAssetsPath();
        public string SettingsFolder();
        public string SaveFolder();
    }
}
