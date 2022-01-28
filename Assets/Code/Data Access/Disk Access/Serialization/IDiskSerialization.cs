namespace DataAccess.DiskAccess.Serialization
{
    public interface IDiskSerialization
    {
        public void SaveToDisk(object dataObject, string filePath, string fileName);
        public T GetDataObjectFromFile<T>(string filePath, string fileName)where T : class;
        public bool FileExists(string filePath, string fileName);
        public void CreateFileFromStreamingAssets<T>(string sourcePathAtStreamingAssets, string sourceFileName, string destinationPath, string destinationFileName);
        public void CopyFileData(string sourcePath, string sourceFileName, string destinationPath, string destinationFileName);        
    }
}