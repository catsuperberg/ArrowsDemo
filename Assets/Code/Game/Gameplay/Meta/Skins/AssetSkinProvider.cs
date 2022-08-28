using AssetScripts.AssetCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Utils;


namespace Game.Gameplay.Meta.Skins
{
    public class AssetSkinProvider<T> : ISkinProvider where T : ISkinData<T>, ISkinData
    {
        public IList<string> Names {get => _permanentSkinData.Select(instance => instance.Name).ToList().AsReadOnly();}        
        IList<T> _permanentSkinData;

        public AssetSkinProvider(IList<T> permanentSkinData)
        {
            if(permanentSkinData == null || !permanentSkinData.Any())
                throw new ArgumentNullException("List<ProjectileSkinData> not provided or empty at: " + this.GetType().Name);
            
            _permanentSkinData = permanentSkinData;
        }

        public UnityEngine.Object LoadResource(string name)
        {
            var skinData = GetSkinIfValid(name);
            return Resources.Load(skinData.PrefabPath.GetResourcesOnlyPath().GetPathWithoutExtension());
        }
        
        public Sprite Icon(string name)
        {
            var skinData = GetSkinIfValid(name);
            return Resources.Load<Sprite>(skinData.IconPath.GetResourcesOnlyPath().GetPathWithoutExtension());
        }    
        
        public BigInteger Price(string name)
        {
            var skinData = GetSkinIfValid(name);
            return skinData.BaseCost ?? throw new Exception("Skin cost is null during execution");            
        }    
        
        T GetSkinIfValid(string name)
        {            
            var skinData = _permanentSkinData.First(instance => instance.Name == name);
            if(skinData == null)
                throw new Exception("Invalid name provided to instantiate a skin: "+ name);
                
            return skinData;
        }
    }
}