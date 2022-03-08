using System.IO;

namespace DataManagement
{
    public class DiskAcessor : IEntryAccessor
    {
        public bool EntyExists(string entryName)
        {
            UnityEngine.Debug.LogWarning("Looking for: " + entryName);
            UnityEngine.Debug.LogWarning("It exists: " + File.Exists(entryName));
            return File.Exists(entryName);
        }
        
        public string ReadString(string entryName)
        {
            if(!File.Exists(entryName))
                throw new FileLoadException("No file found at: " + entryName);
                
            return File.ReadAllText(entryName);
        }
        
        public void WriteString(string entryName, string data)
        {
            UnityEngine.Debug.Log("Writing entry at: " + entryName);
            UnityEngine.Debug.Log("Data to write:");            
            UnityEngine.Debug.Log(data);
            File.WriteAllText(entryName, data);
        }
    }
}