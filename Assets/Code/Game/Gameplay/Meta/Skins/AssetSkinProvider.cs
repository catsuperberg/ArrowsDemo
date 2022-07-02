using AssetScripts.AssetCreation;
using DataManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Zenject;

using Vector2 = UnityEngine.Vector2;

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
            var texture = Resources.Load(skinData.IconPath) as Texture2D;
            return Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), new Vector2(texture.width / 2, texture.height / 2));
        }        
        
        public BigInteger Price(string name)
        {
            var skinData = GetSkinIfValid(name);
            return skinData.BaseCost;            
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