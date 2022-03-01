using System.IO;
using UnityEngine;

namespace DataAccess.DiskAccess.GameFolders
{
    public class AndroidStreamingAssets : IStreamingAssetsReader
    {
        public string GetTextFromFile(string pathAtStreamingAssets)
        {
            var fullPath = Path.Combine(Application.streamingAssetsPath, pathAtStreamingAssets);
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(fullPath);
            www.SendWebRequest();
            while (!www.isDone)
            {
            }
            if(www.downloadHandler.text == "")
                throw new System.Exception("No text was found at StreamingAssets file: " + fullPath);
            return www.downloadHandler.text;
        }
    }
}