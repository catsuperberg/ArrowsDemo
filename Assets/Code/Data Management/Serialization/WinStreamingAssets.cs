using System.IO;
using UnityEngine;
using DataAccess.DiskAccess.GameFolders;

namespace DataAccess.DiskAccess.Serialization
{
    public class WinStreamingAssets : IStreamingAssetsReader
    {
        public string GetTextFromFile(string pathAtStreamingAssets)
        {
            var fullPath = Path.Combine(Application.streamingAssetsPath, pathAtStreamingAssets);
            if(File.Exists(fullPath))
            {
                FileStream stream = new FileStream(fullPath, FileMode.Open);
                string textFromFile;
                using(var sr = new StreamReader(stream)){
                    textFromFile = sr.ReadToEnd();
                    sr.Close();
                }
                stream.Close();
                return textFromFile;
            }
            else
                return "";
        }
    }
}