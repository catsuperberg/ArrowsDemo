using Game.Gameplay.Meta.Skins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public class CrossbowAssetMaker : AssetMaker, IAssetMaker<CrossbowSkinData>
    {        
        (string injestFolder, string outputFolder) _folders;           
        
        public CrossbowAssetMaker(
            RawModelLoader modelLoader, ISkinPrefabGenerator prefabGenerator, PrefabIconGenerator iconGenerator, 
            (string injestFolder, string outputFolder) InOutFolders)
            :base(modelLoader, prefabGenerator, iconGenerator, InOutFolders)
        {            
        }
        
        public IEnumerable<CrossbowSkinData> MakeAssetsFromInjestable()
        {            
            ConvertModelsToAssets();
            return CreateDataEntriesForAssets();
        }
        
        IEnumerable<CrossbowSkinData> CreateDataEntriesForAssets()
            => SkinFolders.Select(entry => AssignAssetPath(entry));

        CrossbowSkinData AssignAssetPath(string assetFolder)
        {
            var name = new DirectoryInfo(Path.GetFileName(assetFolder)).Name;
            var prefabPath = Directory.GetFiles(assetFolder, "*.prefab").First();
            var iconPath = Directory.GetFiles(assetFolder, "icon_*").First();
            return new CrossbowSkinData(name,prefabPath,iconPath, null, null);
        }
    }
}