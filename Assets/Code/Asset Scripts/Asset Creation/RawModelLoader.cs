using DataAccess.DiskAccess.GameFolders;
using Siccity.GLTFUtility;
using System;
using System.Collections;
using System.Collections.Generic;
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
        
        public async Task<List<GameObject>> CreateInactiveGameObjects(IEnumerable<string> pathsToFiles)
        {
            _skinsBeingImported = pathsToFiles.ToList().Count;
            foreach(var file in pathsToFiles)
                Importer.ImportGLBAsync(file, new ImportSettings(), onFinished: OnFinishAsync);
            
            while(_skinsBeingImported > 0) {await Task.Delay(200);};
            return _skinObjects;
        }      
        
        void OnFinishAsync(GameObject result, AnimationClip[] animations)
        {
            _skinObjects.Add(result);
            Debug.LogWarning("Finished importing " + result.name);
            result.SetActive(false);
            _skinsBeingImported--;
        }
    }
}