using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Utils;

namespace DataAccess.DiskAccess.Serialization
{
    public static class JsonFile
    {
        public static void SaveAsJson(object dataObject, string filePath, string fileName)
        {
            FileStream stream = new FileStream(Path.Combine(filePath, withExtension(fileName)), FileMode.Create);        
            string json = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            // string json = JsonUtility.ToJson(dataObject, prettyPrint: true);
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();        
        }
        
        public static void SaveAsJson<T>(T dataObject, string filePath)
        {
            string json = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            // string json = JsonUtility.ToJson(dataObject, prettyPrint: true);
            if(json == "{}" || json == "[]")
                throw new System.Exception("Empty json generated");
                
            if(File.Exists(filePath))
                File.Delete(filePath);
            FileStream stream = new FileStream(filePath, FileMode.Create);   
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();        
        }
        
        public static T GetObjectFromFile<T>(string filePath, string fileName) where T : class
        {
            var pathToFile = Path.Combine(filePath, withExtension(fileName));
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
                // return JsonUtility.FromJson<T>(json);
            }
            else
            {
                Debug.Log("No file at: " + pathToFile);
                return null;
            }
        }
        
        public static T GetObjectFromFile<T>(string filePath) where T : class
        {
            if(File.Exists(filePath))
            {
                FileStream stream = new FileStream(filePath, FileMode.Open);
                string json;
                using(var sr = new StreamReader(stream)){
                    json = sr.ReadToEnd();
                    sr.Close();
                }
                stream.Close();
                return JsonConvert.DeserializeObject<T>(json);
                // return JsonUtility.FromJson<T>(json);
            }
            else
            {
                Debug.Log("No file at: " + filePath);
                return null;
            }
        }
        
        public static T LoadFromResources<T>(string pathToResource) where T : class
        {
            var json = Resources.Load<TextAsset>(pathToResource.GetAtResourcesWithNoExtension());
            return JsonConvert.DeserializeObject<T>(json.text);  
        }
        
        private static string withExtension(string fileName) => fileName + ".json";
    }
}