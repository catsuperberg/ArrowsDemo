using System.IO;

namespace DataManagement
{
    public class DiskAcessor : IEntryAccessor
    {
        public string ReadString(string entryName)
        {
            if(!File.Exists(entryName))
                throw new FileLoadException("No file found at: " + entryName);
                
            return File.ReadAllText(entryName);
        }
        
        public void WriteString(string entryName, string data)
        {
            File.WriteAllText(entryName, data);
        }
    }
}