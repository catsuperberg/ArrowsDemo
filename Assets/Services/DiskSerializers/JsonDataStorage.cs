using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GameStorage
{
    public class JsonDataStorage : IDiskSerialization
    {
        private readonly IStreamingAssetsReader _streamingAssets;
        
        private const string _fileExtension = ".json";
        
        JsonDataStorage(IStreamingAssetsReader streamingAssetsReader)
        {            
            if(streamingAssetsReader == null)
                throw new System.Exception("IStreamingAssetsReader isn't provided to JsonDataStorage");
                
            _streamingAssets = streamingAssetsReader;
        }    
        
        public void SaveToDisk(object dataObject, string filePath, string fileName)
        {
            FileStream stream = new FileStream(Path.Combine(filePath, withExtension(fileName)), FileMode.Create);        
            string json = JsonUtility.ToJson(dataObject, prettyPrint: true);
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));        
            stream.Close();        
        }
        
        public T GetDataObjectFromFile<T>(string filePath, string fileName) where T : class
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
        
        public bool FileExists(string filePath, string fileName)
        {
            var pathToFile = Path.Combine(filePath, withExtension(fileName));
            return File.Exists(pathToFile);
        }
        
        public void CreateFileFromStreamingAssets<T>(string sourcePathAtStreamingAssets, string sourceFileName, string destinationPath, string destinationFileName)
        {
            var sourceFilePath = Path.Combine(sourcePathAtStreamingAssets, withExtension(sourceFileName));
            var json = _streamingAssets.GetTextFromFile(sourceFilePath);                
            
            FileStream stream = new FileStream(Path.Combine(destinationPath, withExtension(destinationFileName)), FileMode.Create);        
            stream.Write(Encoding.ASCII.GetBytes(json), 0,Encoding.ASCII.GetByteCount(json));            
            stream.Close();
        }        
        
        public void CopyFileData(string sourcePath, string sourceFileName, string destinationPath, string destinationFileName)
        {
            var pathToSourceFile = Path.Combine(sourcePath, withExtension(sourceFileName));
            var pathToOutputFile = Path.Combine(destinationPath, withExtension(destinationFileName));
            if(File.Exists(pathToSourceFile))
                File.Copy(pathToSourceFile, pathToOutputFile, overwrite: true);
            else
                throw new System.Exception("Copy failed, no file found at: " + pathToSourceFile);
        }
        
        private string withExtension(string fileName)
        {
            return fileName + _fileExtension;
        }
    }
}
