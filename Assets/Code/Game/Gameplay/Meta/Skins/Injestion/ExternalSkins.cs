using AssetScripts.AssetCreation;
using DataAccess.DiskAccess.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace Game.Gameplay.Meta.Skins
{
    public class MutableSkinResource
    {
        public UnityEngine.Object resource;
        public Sprite icon;
        public BasicInjestData injestData;
    }
    
    public class ExternalSkins
    {
        ISkinPrefabGenerator _resourceGenerator;
        PrefabIconGenerator _iconGenerator;
        
        public IList<SkinResource> Skins {get => _skins.AsReadOnly();}
        List<SkinResource> _skins;
        
        IEnumerable<GameObject> _instantiatedSkins;
        IEnumerable<(string name, string pathToModel)> _modelsToLoad;
        IEnumerable<(string name, string pathToData)> _injestDataPaths;
        List<(string name, MutableSkinResource data)> _tempResources;
        
        public ExternalSkins(ISkinPrefabGenerator resourceGenerator, PrefabIconGenerator iconGenerator, string pathToScanForSkins)
        {
            _resourceGenerator = resourceGenerator ?? throw new ArgumentNullException(nameof(resourceGenerator));
            _iconGenerator = iconGenerator ?? throw new ArgumentNullException(nameof(iconGenerator));
            
            
            try {Directory.GetFiles(pathToScanForSkins, "*.glb");} // HACK for some reason Directory.Exists(pathToScanForSkins) always returns false
            catch
            {
                SetSkinsEmpty();
                return;
            }
            
            ScanForModelsToInjest(pathToScanForSkins);
            ScanForDataToInjest(pathToScanForSkins);
            _instantiatedSkins = new RawModelLoader().CreateInactiveGameObjects(_modelsToLoad);
            CreateEmptyResourcesForEveryModel();
            AddPrefabResources();
            AddIcons();
            LoadInjestData();
            AssembleSkins();
        }
        
        void SetSkinsEmpty()
        {
            _skins = new List<SkinResource>();
        }
        
        void ScanForModelsToInjest(string folderToScan)
        {
            var skinFolders = Directory.GetDirectories(folderToScan).Where(entry => Directory.GetFiles(entry, "*.glb").Any());
            _modelsToLoad = skinFolders
                .Select(entry => (name: new DirectoryInfo(entry).Name, pathToModel: Directory.GetFiles(entry, "*.glb").FirstOrDefault()));
        }  
        
        void  ScanForDataToInjest(string folderToScan)
        {
            var skinDataFolders = Directory.GetDirectories(folderToScan).Where(entry => Directory.GetFiles(entry, "injestData.*").Any());
            _injestDataPaths = skinDataFolders
                .Select(entry => (name: new DirectoryInfo(entry).Name, pathToData: Directory.GetFiles(entry, "injestData.*").FirstOrDefault()));
        }
        
        void CreateEmptyResourcesForEveryModel()
            => _tempResources = _modelsToLoad.Select(entry => (name: entry.name, data: new MutableSkinResource())).ToList();
        
        void AddPrefabResources()
            =>_tempResources.ToList()
                .ForEach(entry => 
                    entry.data.resource = _resourceGenerator.CreateRuntimeResource(_instantiatedSkins.First(skin => skin.name == entry.name) ??
                        throw new NullReferenceException("Resource generator created null resource")));
        
        void AddIcons()
        {
            _instantiatedSkins.ToList().ForEach(entry => entry.SetActive(true));
            var iconTextures =_tempResources.ToList()
                .Select(entry => (name: entry.name, texture: _iconGenerator.CreatePrefabPreview(_instantiatedSkins.First(skin => skin.name == entry.name))));
            _tempResources.ToList()
                .ForEach(entry => 
                    entry.data.icon = TextureToSprite(iconTextures.First(tex => tex.name == entry.name).texture));    
        }
        
        Sprite TextureToSprite(Texture2D texture)
            => Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.zero,100);
        
        void LoadInjestData()
            => _tempResources.ToList()
                .ForEach(entry => 
                    entry.data.injestData = LoadDataOrDefault(_injestDataPaths.First(data => data.name == entry.name).pathToData));
        
        BasicInjestData LoadDataOrDefault(string pathToData)
            => JsonFile.GetObjectFromFile<BasicInjestData>(pathToData) ?? new BasicInjestData();
        
        void AssembleSkins()
        {            
            _skins = _tempResources.Select(entry => new SkinResource(
                    name: entry.name,
                    gameObjectResourse: entry.data.resource,
                    icon: entry.data.icon,
                    price: entry.data.injestData.BaseCost,
                    adWatchRequired: entry.data.injestData.AdWatchRequired)).ToList();
        }
    }
}