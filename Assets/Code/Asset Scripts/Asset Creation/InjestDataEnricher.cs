using DataAccess.DiskAccess.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetScripts.AssetCreation
{
    public class InjestDataEnricher<TOne, TTwo> 
        where TOne : ISkinDataEnricher<TOne, TTwo> 
        where TTwo : BasicInjestData
    {                   
        TOne hackInstanceToCall = (TOne)Activator.CreateInstance(typeof(TOne)); // HACK can't access static method for generic type
            
        public List<TOne> GetInjestDataForAllSkins(IEnumerable<TOne> skinsWithAssetPaths, IEnumerable<(string name, string pathToData)> skinInjestDataPaths)
        {            
            var data = new List<TOne>();
            var fullDataSkinsRaw = skinsWithAssetPaths.Where(entry => skinInjestDataPaths.Any(skin => skin.name == entry.Name));
            var modelOnlySkinsRaw = skinsWithAssetPaths.Except(fullDataSkinsRaw);
            var dataOnlySkins = skinInjestDataPaths.Where(entry => !fullDataSkinsRaw.Any(skin => skin.Name == entry.name))
                .Select(entry => hackInstanceToCall.ToSkinData(entry.name, LoadInjestData(entry.pathToData)));            
            var fullDataSkins = fullDataSkinsRaw
                .Select(entry => entry.EnrichWithInjestData(
                    LoadInjestData(skinInjestDataPaths.First(skin => skin.name == entry.Name).pathToData)));
            var modelOnlySkins = modelOnlySkinsRaw   
                .Select(entry => entry.EnrichWithDefaultValues());
                
            data.AddRange(modelOnlySkins);    
            data.AddRange(dataOnlySkins);          
            data.AddRange(fullDataSkins);  
            return data;
        }
        
        TTwo LoadInjestData(string path)
            => JsonFile.GetObjectFromFile<TTwo>(path);
    }
}