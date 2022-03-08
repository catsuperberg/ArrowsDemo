namespace DataManagement
{
    public interface INonVolatileStorage
    {
        public bool EntryExists(string entryName);
         public T ReadEntry<T>(string entryName);
         public void WriteEntry<T>(string entryName, T objectToStore);
    }
}