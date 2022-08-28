using DataAccess.DiskAccess.Serialization;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AssetScripts.AssetCreation
{
    public class InjestDataEnricher<TOne, TTwo> 
        where TOne : ISkinDataEnricher<TOne, TTwo> 
        where TTwo : BasicInjestData
    {                   
        TOne _hackInstanceToCall = (TOne)Activator.CreateInstance(typeof(TOne)); // HACK can't access static method for generic type
            
        IEnumerable<(string name, string pathToData)> _skinInjestDataPaths;
            
        public InjestDataEnricher(string folderOfFoldersWithInjestData)
        {
            _skinInjestDataPaths = ScanForDataToInjest(folderOfFoldersWithInjestData);
        }    
        
        protected IEnumerable<(string name, string pathToData)> ScanForDataToInjest(string injestFolder)
        {
            var skinDataFolders = Directory.GetDirectories(injestFolder).Where(entry => Directory.GetFiles(entry, "injestData.*").Any());
            return skinDataFolders
                .Select(entry => (name: new DirectoryInfo(entry).Name, pathToData: Directory.GetFiles(entry, "injestData.*").FirstOrDefault()));
        }
            
        public List<TOne> GetInjestDataForAllSkins(IEnumerable<TOne> skinsWithAssetPaths)
        {            
            var fullDataSkinsRaw = skinsWithAssetPaths.Where(entry => _skinInjestDataPaths.Any(skin => skin.name == entry.Name));
            var modelOnlySkinsRaw = skinsWithAssetPaths.Except(fullDataSkinsRaw);
            
            var dataOnlySkins = _skinInjestDataPaths.Where(entry => !fullDataSkinsRaw.Any(skin => skin.Name == entry.name))
                .Select(entry => _hackInstanceToCall.ToSkinData(entry.name, LoadInjestData(entry.pathToData)));            
            var fullDataSkins = fullDataSkinsRaw
                .Select(entry => entry.EnrichWithInjestData(
                    LoadInjestData(_skinInjestDataPaths.First(skin => skin.name == entry.Name).pathToData)));
            var modelOnlySkins = modelOnlySkinsRaw   
                .Select(entry => entry.EnrichWithDefaultValues());
                
            var data = new List<TOne>();
            data.AddRange(modelOnlySkins);    
            data.AddRange(dataOnlySkins);          
            data.AddRange(fullDataSkins);  
            return data;
        }
        
        TTwo LoadInjestData(string path)
            => JsonFile.GetObjectFromFile<TTwo>(path);
    }
}