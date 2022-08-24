using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetScripts.AssetCreation
{
    public interface IAssetMaker<T>
    {
        /// <summary>
        /// returns list of SkinData with paths to assets filled
        /// </summary>
        public IEnumerable<T> MakeAssetsFromInjestable(); 
    }
    
    public abstract class AssetMaker
    {        
        protected (string injestFolder, string outputFolder) _folders;
        
        RawModelLoader _modelLoader;
        ISkinPrefabGenerator _prefabGenerator;
        PrefabIconGenerator _iconGenerator;       
        
        IEnumerable<(string name, string pathToModel)> _modelsToSave;

        public AssetMaker(
            RawModelLoader modelLoader, ISkinPrefabGenerator prefabGenerator, PrefabIconGenerator iconGenerator, 
            (string injestFolder, string outputFolder) InOutFolders)
        {
            _modelLoader = modelLoader ?? throw new ArgumentNullException(nameof(modelLoader));
            _prefabGenerator = prefabGenerator ?? throw new ArgumentNullException(nameof(prefabGenerator));
            _iconGenerator = iconGenerator ?? throw new ArgumentNullException(nameof(iconGenerator));
            _folders = InOutFolders;
        }

        protected void ConvertModelsToAssets()
        {
            _modelsToSave = ScanForModelsToInjest();
            SavePrefabsWithAssets();
            GenerateIconFiles();
        }
        
        protected IEnumerable<(string name, string pathToModel)> ScanForModelsToInjest()
        {
            var skinFolders = Directory.GetDirectories(_folders.injestFolder).Where(entry => Directory.GetFiles(entry, "*.glb").Any());
            return skinFolders
                .Select(entry => (name: new DirectoryInfo(entry).Name, pathToModel: Directory.GetFiles(entry, "*.glb").FirstOrDefault()));
        }
        
        protected void SavePrefabsWithAssets()
        {
            var modelsInScene = _modelLoader.CreateInactiveGameObjects(_modelsToSave);
            _modelsToSave.ToList().ForEach(entry => Directory.CreateDirectory(Path.Combine(_folders.outputFolder, entry.name)));
            _modelsToSave.ToList().ForEach(entry => _prefabGenerator.CreatePrefab(
                modelsInScene.FirstOrDefault(GO => GO.name == entry.name), Path.Combine(_folders.outputFolder, entry.name)));
            modelsInScene.ForEach(GameObject.DestroyImmediate);
        }
        
        protected void GenerateIconFiles()
        {
            var iconsToSave = _modelsToSave.Select(entry => 
                (name: entry.name, texture: _iconGenerator.CreatePrefabPreview(LoadPrefabByName(entry.name))));
            iconsToSave.ToList().ForEach(entry => SaveAsSpriteAsset(entry.texture, Path.Combine(_folders.outputFolder, Path.Combine(entry.name, "icon_" + entry.name + ".asset"))));            
        }
        
        protected void SaveAsSpriteAsset(Texture2D texture, string pathToSaveAt)
        {
            var sprite = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.zero,100);
            AssetDatabase.CreateAsset(texture, pathToSaveAt);
            AssetDatabase.AddObjectToAsset(sprite, pathToSaveAt);
        }
        
        GameObject LoadPrefabByName(string name) => Resources.Load<GameObject>(Path.Combine(name, name));
        
        protected List<string> SkinFolders
            => _modelsToSave.Select(entry => Path.Combine(_folders.outputFolder, entry.name)).ToList();
    }
}