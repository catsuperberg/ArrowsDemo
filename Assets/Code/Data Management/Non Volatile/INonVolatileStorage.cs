namespace DataManagement
{
    public interface INonVolatileStorage
    {
         public T ReadEntry<T>(string entryName);
         public void WriteEntry<T>(string entryName, T objectToStore);
    }
}