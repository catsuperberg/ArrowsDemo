namespace DataManagement
{
    public interface IEntryAccessor
    {
        public string ReadString(string entryName);        
        public void WriteString(string entryName, string data);
    }
}