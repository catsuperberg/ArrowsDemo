using DataAccess.DiskAccess.GameFolders;
using Siccity.GLTFUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public class RawModelLoader
    {        
        List<GameObject> _skinObjects = new List<GameObject>();
        int _skinsBeingImported = 0;
        
        
        public async Task<List<GameObject>> CreateInactiveGameObjectsAsync(IEnumerable<string> pathsToFiles)
            => await CreateInactiveGameObjectsAsync(DirectionsWithNamesOfFiles(pathsToFiles));
        
        public List<GameObject> CreateInactiveGameObjects(IEnumerable<string> pathsToFiles)
            => CreateInactiveGameObjects(DirectionsWithNamesOfFiles(pathsToFiles));
        
        IEnumerable<(string name, string path)> DirectionsWithNamesOfFiles(IEnumerable<string> pathsToFiles) 
            => pathsToFiles.Select(entry => (name: Path.GetFileNameWithoutExtension(entry), path: entry));
        
        public async Task<List<GameObject>> CreateInactiveGameObjectsAsync(IEnumerable<(string name, string path)> fileDirections)
        {
            _skinObjects = new List<GameObject>();
            _skinsBeingImported = fileDirections.ToList().Count;
            foreach(var file in fileDirections)
                Importer.ImportGLBAsync(file.path, new ImportSettings(), onFinished: 
                    (result, animations) => {OnFinishAsync(result, animations, file.name);});
            
            while(_skinsBeingImported > 0) {await Task.Delay(200);};
            return _skinObjects;
        }
        
        public List<GameObject> CreateInactiveGameObjects(IEnumerable<(string name, string path)> fileDirections)
        {
            var skins = new List<GameObject>();
            foreach(var file in fileDirections)
                skins.Add(LoadFromFile(file.path, file.name));
            
            return skins;
        }  
        
        GameObject LoadFromFile(string pathToFile, string name)
        {
            var GO = Importer.LoadFromFile(pathToFile);
            GO.name = name;
            GO.SetActive(false);
            return GO;
        }  
        
        void OnFinishAsync(GameObject result, AnimationClip[] animations, string objectName)
        {
            result.name = objectName;
            _skinObjects.Add(result);
            result.SetActive(false);
            _skinsBeingImported--;
        }
    }
}