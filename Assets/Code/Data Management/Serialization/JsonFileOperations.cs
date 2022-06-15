using System.IO;
using System.Text;
using UnityEngine;

namespace DataAccess.DiskAccess.Serialization
{
    public static class JsonFileOperations
    {
        public static void SaveAsJson(object dataObject, string filePath, string fileName)
        {
            FileStream stream = new FileStream(Path.Combine(filePath, withExtension(fileName)), FileMode.Create);        
            string json = JsonUtility.ToJson(dataObject, prettyPrint: true);
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();        
        }
        
        public static void SaveAsJson(object dataObject, string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Create);        
            string json = JsonUtility.ToJson(dataObject, prettyPrint: true);
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();        
        }
        
        public static T GetDataObjectFromJsonFile<T>(string filePath, string fileName) where T : class
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
                return JsonUtility.FromJson<T>(json);
            }
            else
            {
                Debug.Log("No file at: " + pathToFile);
                return null;
            }
        }
        
        public static T GetDataObjectFromJsonFile<T>(string filePath) where T : class
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
                return JsonUtility.FromJson<T>(json);
            }
            else
            {
                Debug.Log("No file at: " + filePath);
                return null;
            }
        }
        
        private static string withExtension(string fileName) => fileName + ".json";
    }
}