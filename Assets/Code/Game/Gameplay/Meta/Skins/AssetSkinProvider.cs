using AssetScripts.AssetCreation;
using DataManagement;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Meta.Skins
{
    public class AssetSkinProvider : ISkinProvider
    {
        public IList<string> Names {get => _permanentSkinData.Select(instance => instance.Name).ToList().AsReadOnly();}        
        IList<ProjectileSkinData> _permanentSkinData;

        public AssetSkinProvider(IList<ProjectileSkinData> permanentSkinData)
        {
            if(permanentSkinData == null || !permanentSkinData.Any())
                throw new ArgumentNullException("List<ProjectileSkinData> not provided or empty at: " + this.GetType().Name);
            
            _permanentSkinData = permanentSkinData;
        }

        public UnityEngine.Object LoadResource(string name)
        {
            var skinData = GetSkinIfValid(name);
            return Resources.Load(skinData.PrefabPath);
        }
        
        public Sprite Icon(string name)
        {
            var skinData = GetSkinIfValid(name);
            return Resources.Load(skinData.IconPath) as Sprite;
        }        
        
        ProjectileSkinData GetSkinIfValid(string name)
        {            
            var skinData = _permanentSkinData.First(instance => instance.Name == name);
            if(skinData == null)
                throw new Exception("Invalid name provided to instantiate a skin: "+ name);
                
            return skinData;
        }
    }
}