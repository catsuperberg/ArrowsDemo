using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine;
using Utils;

namespace DataAccess.DiskAccess.Serialization
{
    public static class JsonFile
    {
        public static string ToJson(object dataObject)
            => JsonConvert.SerializeObject(dataObject, Formatting.Indented); 
        
        public static void SaveAsJson(object dataObject, string folderPath, string fileName)
        {
            FileStream stream = new FileStream(Path.Combine(folderPath, withExtension(fileName)), FileMode.Create);        
            string json = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();        
        }
        
        public static void SaveAsJson<T>(T dataObject, string filePath)
        {
            var pathToFile = withExtension(filePath.GetPathWithoutExtension());
            string json = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            if(json == "{}" || json == "[]")
                throw new System.Exception("Empty json generated");
                
            if(File.Exists(pathToFile))
                File.Delete(pathToFile);
            FileStream stream = new FileStream(pathToFile, FileMode.Create);   
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();        
        }
        
        public static T GetObjectFromFile<T>(string folderPath, string fileName) where T : class
        {            
            var pathToFile = Path.Combine(folderPath, withExtension(fileName.GetPathWithoutExtension()));
            if(File.Exists(pathToFile))
            {
                FileStream stream = new FileStream(pathToFile, FileMode.Open);
                string json;
                using(var sr = new StreamReader(stream)){
                    json = sr.ReadToEnd();
                    sr.Close();
                }
                stream.Close();
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                Debug.Log("No file at: " + pathToFile);
                return null;
            }
        }
        
        public static T GetObjectFromFile<T>(string filePath) where T : class
        {
            var pathToFile = withExtension(filePath.GetPathWithoutExtension());
            if(File.Exists(pathToFile))
            {
                FileStream stream = new FileStream(pathToFile, FileMode.Open);
                string json;
                using(var sr = new StreamReader(stream)){
                    json = sr.ReadToEnd();
                    sr.Close();
                }
                stream.Close();
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                Debug.Log("No file at: " + pathToFile);
                return null;
            }
        }
        
        public static T LoadFromResources<T>(string filePath) where T : class
        {            
            var tempString = filePath.GetAtResourcesWithNoExtension();
            var json = Resources.Load<TextAsset>(filePath.GetAtResourcesWithNoExtension());
            return (json?.text != null) ? JsonConvert.DeserializeObject<T>(json.text) : null;  
        }
        
        public static T LoadFromResources<T>(string fullPath, string fileName) where T : class
        {            
            var filePath = Path.Combine(fullPath, fileName);
            return LoadFromResources<T>(filePath); 
        }
        
        #if UNITY_EDITOR
        
        public static T LoadFromResourcesAnyThread<T>(string filePath) where T : class
        {            
            return GetObjectFromFile<T>(filePath);  
        }
        
        public static T LoadFromResourcesAnyThread<T>(string fullPath, string fileName) where T : class
        {            
            var filePath = Path.Combine(fullPath, fileName);
            return LoadFromResourcesAnyThread<T>(filePath); 
        }
        
        #endif
        
        private static string withExtension(string fileName) => fileName + ".json";
    }     
}