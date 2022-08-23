using System;
using System.Numerics;

namespace AssetScripts.AssetCreation
{
    public interface ISkinData<TOne>
    {
        string Name {get;}
        string PrefabPath {get;}
        string IconPath {get;}
        BigInteger? BaseCost {get;}
        bool? AdWatchRequired {get;}
        TOne GetNewWithUpdatedValues(TOne data);
        TOne EnrichWithDefaultValues();
    }
    
    public interface ISkinDataEnricher<TOne, TTwo> : ISkinData<TOne>
    {
        TOne EnrichWithInjestData(TTwo injestData);
        TOne ToSkinData(string name, TTwo injestData);
    }
}