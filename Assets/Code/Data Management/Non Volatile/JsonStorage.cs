using Newtonsoft.Json;

namespace DataManagement
{
    public class JsonStorage : INonVolatileStorage
    {
        IEntryAccessor _entryAccessor;
        string fileExtension = ".json";
        
        public JsonStorage(IEntryAccessor entryAccessor)
        {
            if(entryAccessor == null)
                throw new System.Exception("No infrastructure provided for JsonStorage");
            
            _entryAccessor = entryAccessor;
        }
        
        public T ReadEntry<T>(string entryName)
        {
            var jsonString = _entryAccessor.ReadString(entryName + fileExtension);    
            return JsonConvert.DeserializeObject<T>(jsonString); 
        }
        
        public void WriteEntry<T>(string entryName, T objectToStore)
        {
            var jsonString = JsonConvert.SerializeObject(objectToStore, Formatting.Indented);       
            _entryAccessor.WriteString(entryName + fileExtension, jsonString);
        }
    }
}