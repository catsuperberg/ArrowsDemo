using System.IO;
using UnityEngine;

namespace GameStorage
{
    public class AndroidStreamingAssets : IStreamingAssetsReader
    {
        public string GetTextFromFile(string pathAtStreamingAssets)
        {
            var fullPath = Path.Combine(Application.streamingAssetsPath, pathAtStreamingAssets);
            if(File.Exists(fullPath))
            {
                UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(fullPath);
                www.SendWebRequest();
                while (!www.isDone)
                {
                }
                return www.downloadHandler.text;
            }
            else
                return "";
        }
    }
}