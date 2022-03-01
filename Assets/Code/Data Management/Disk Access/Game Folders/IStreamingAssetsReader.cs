namespace DataAccess.DiskAccess.GameFolders
{
    public interface IStreamingAssetsReader
    {
        public string GetTextFromFile(string pathAtStreamingAssets);
    }
}
