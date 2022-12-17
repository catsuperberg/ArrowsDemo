namespace DataAccess.DiskAccess.GameFolders
{
    public interface IGameFolders
    {
        public string StreamingAssetsPath {get;}
        public string SettingsFolder {get;}
        public string SaveFolder {get;}
        public string AssetInjest {get;}
        
        public string ResourcesGameBalance {get;}
        public string WithFullBasePath(string path);
    }
}
