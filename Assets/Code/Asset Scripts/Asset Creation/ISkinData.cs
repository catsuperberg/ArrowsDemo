using System;
using System.Numerics;

namespace AssetScripts.AssetCreation
{
    public interface ISkinData<T>
    {
        public string Name {get;}
        public string PrefabPath {get;}
        public string IconPath {get;}
        public BigInteger? BaseCost {get;}
        public bool? AdWatchRequired {get;}
        
        public T GetNewWithUpdatedValues(T data);
        public T EnrichWithDefaultValues();
    }
}