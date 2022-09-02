using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;


namespace Game.Gameplay.Meta.Skins
{
    public class ExternalSkinProvider : ISkinProvider 
    {
        public IList<string> Names {get => _skins.Select(entry => entry.Name).ToList();}   
        IList<SkinResource> _skins;   

        public ExternalSkinProvider(IList<SkinResource> skins)
        {
            _skins = skins ?? throw new ArgumentNullException(nameof(skins));
        }

        public UnityEngine.Object LoadResource(string name)
            => GetSkinIfValid(name).GameObjectResourse;
        
        public Sprite Icon(string name)
            => GetSkinIfValid(name).Icon;
        
        public BigInteger Price(string name)
            => GetSkinIfValid(name).Price;
        
        SkinResource GetSkinIfValid(string name)
        {            
            var skinData = _skins.First(instance => instance.Name == name);
            if(skinData == null)
                throw new Exception("Invalid name provided to instantiate a skin: "+ name);
                
            return skinData;
        }
    }
}