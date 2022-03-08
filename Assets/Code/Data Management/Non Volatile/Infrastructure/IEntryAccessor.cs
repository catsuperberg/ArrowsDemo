namespace DataManagement
{
    public interface IEntryAccessor
    {
        public bool EntyExists(string entryName);
        public string ReadString(string entryName);        
        public void WriteString(string entryName, string data);
    }
}