using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading;
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
            string json = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            if(json == "{}" || json == "[]")
                throw new System.Exception("Empty json generated");
                
            if(File.Exists(filePath))
                File.Delete(filePath);
            FileStream stream = new FileStream(filePath, FileMode.Create);   
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
            }
            else
            {
                Debug.Log("No file at: " + filePath);
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
        
        // public static T LoadFromResourcesOnMainThread<T>(string filePath) where T : class
        // {            
        //     TextAsset json = null;
        //     // var semaphore = new SemaphoreSlim(0,1);
        //     // UnityMainThreadDispatcher.Instance().Enqueue(() => {
        //     //     Resources.LoadAsync
        //     //     json = Resources.Load<TextAsset>(filePath.GetAtResourcesWithNoExtension());
        //     //     semaphore.Release();
        //     // });
        //     // semaphore.Wait();
        //     var jsonRequest = Resources.LoadAsync<TextAsset>(filePath.GetAtResourcesWithNoExtension());
        //     while(!jsonRequest.isDone){}
        //     json = jsonRequest.asset as TextAsset;
        //     return (json?.text != null) ? JsonConvert.DeserializeObject<T>(json.text) : null;  
        // }
        
        private static string withExtension(string fileName) => fileName + ".json";
    }//"Assets/Code/Game/Game Design/Resources/Game Balance/GeneratedOperationFrequencies"
    
}