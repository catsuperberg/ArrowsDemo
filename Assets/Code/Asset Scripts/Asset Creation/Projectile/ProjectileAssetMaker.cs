using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using Utils;

namespace AssetScripts.AssetCreation
{
    public class ProjectileAssetMaker : AssetMaker, IAssetMaker<ProjectileSkinData>
    {        
        public ProjectileAssetMaker(
            RawModelLoader modelLoader, ISkinPrefabGenerator prefabGenerator, PrefabIconGenerator iconGenerator, 
            (string injestFolder, string outputFolder) InOutFolders)
            :base(modelLoader, prefabGenerator, iconGenerator, InOutFolders)
        {            
        }
        
        public IEnumerable<ProjectileSkinData> MakeAssetsFromInjestable()
        {            
            ConvertModelsToAssets();
            return CreateDataEntriesForAssets();
        }
        
        IEnumerable<ProjectileSkinData> CreateDataEntriesForAssets()
            => SkinFolders.Select(entry => AssignAssetPath(entry));

        ProjectileSkinData AssignAssetPath(string assetFolder)
        {
            var name = new DirectoryInfo(Path.GetFileName(assetFolder)).Name;
            var prefabPath = Directory.GetFiles(assetFolder, "Bundle_*.prefab").First().ReplaceBackSlashes();
            var iconPath = Directory.GetFiles(assetFolder, "icon_*").First().ReplaceBackSlashes();
            return new ProjectileSkinData(name,prefabPath,iconPath, null, null);
        }
    }
}