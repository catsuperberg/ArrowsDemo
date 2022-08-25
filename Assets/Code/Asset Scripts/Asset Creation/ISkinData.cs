using System;
using System.Numerics;

namespace AssetScripts.AssetCreation
{
    public interface ISkinData
    {
        string Name {get;}
        string PrefabPath {get;}
        string IconPath {get;}
        BigInteger? BaseCost {get;}
        bool? AdWatchRequired {get;}
    }
    
    public interface ISkinData<TOne> : ISkinData
    {
        TOne GetNewWithUpdatedValues(TOne data);
        TOne EnrichWithDefaultValues();
    }
    
    public interface ISkinDataEnricher<TOne, TTwo> : ISkinData<TOne>
    {
        TOne EnrichWithInjestData(TTwo injestData);
        TOne ToSkinData(string name, TTwo injestData);
    }
}