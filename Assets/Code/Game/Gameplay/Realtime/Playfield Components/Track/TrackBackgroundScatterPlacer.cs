using AssetScripts.Instantiation;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using Game.Gameplay.Realtime.PlayfieldComponents.Track.TrackItems;
using GameMath;
using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public class TrackBackgroundScatterPlacer
    {      
        enum Side{Left,Right};
        
        Spline _track;
        Spline _groupsOfPrefabsToSpread;
        IEnumerable<IEnumerable<GameObject>> _gproupsOfAssets;
        (float dencityCoefficient, float width) _parameters;        
        
        const float _islandControllArea = 350;
        const float _runUpLength = 70;
        const float _scatterHeight = -60;
        (float min, float max) _angleRange {get => (20, 160);} 
        float _outerEdge => _parameters.width;        
        float _closerEdge => _outerEdge * 0.25f;   
        float _distanceBetweenIslands => 100 / _parameters.dencityCoefficient;     
        float _trackEndZ;
        IEnumerable<Dictionary<GameObject, float>> _gproupsOfAssetsWithRadii;
        
        public TrackBackgroundScatterPlacer(
            IEnumerable<IEnumerable<GameObject>> gproupsOfAssets, Spline track, 
            (float dencityCoefficient, float width) parameters)
        {
            _gproupsOfAssets = gproupsOfAssets ?? throw new System.ArgumentNullException(nameof(gproupsOfAssets));
            _track = track ?? throw new System.ArgumentNullException(nameof(track));
            _parameters = parameters;
            
            _trackEndZ = _track.GetSampleAtDistance(_track.Length - 0.01f).location.z;      
            Debug.LogWarning($"track edn Z: {_trackEndZ}");        
            _gproupsOfAssetsWithRadii = _gproupsOfAssets.Select(entry => EnrichWithRadius(entry));;
        }
        
        Dictionary<GameObject, float> EnrichWithRadius(IEnumerable<GameObject> prefabs)
            => prefabs.ToDictionary(entry => entry, entry => 
                entry.GetComponent<Renderer>().bounds.extents.magnitude);// * entry.GetComponent<Transform>().localScale.x);
        
        public List<(GameObject prefab, Vector3 position)> SpreadAssets()
        {              
            var result = new List<(GameObject prefab, Vector3 position)>();                   
            result.AddRange(ScatterAlongTheTrack(Side.Left).ToList());               
            result.AddRange(ScatterAlongTheTrack(Side.Right).ToList());  
            return result;
        }        
        
        List<(GameObject prefab, Vector3 position)> ScatterAlongTheTrack(Side side)
        {
            var scattered = new List<(GameObject prefab, Vector3 position)>();
            do
            {                                
                scattered.AddRange(GenerateIsland(scattered));
            } while(!EndRiched(scattered));
            scattered.ForEach(entry => Debug.Log(entry.position.x)); 
            if(side == Side.Left)
                scattered = scattered
                                .Select(entry => (prefab: entry.prefab, position: entry.position - new Vector3(_outerEdge+_closerEdge,0,0)))
                                .ToList();            
            
            scattered.ForEach(entry => Debug.LogWarning(entry.position.x)); 
            return scattered;
        }
        
        bool EndRiched(List<(GameObject prefab, Vector3 position)> scattered)
            => FurthestCenter(scattered).z >= (_trackEndZ - _runUpLength*2);
        
        Vector3 FurthestCenter(List<(GameObject prefab, Vector3 position)> scattered)
            => scattered.OrderByDescending(entry => entry.position.z).FirstOrDefault().position;
            // => scattered.Aggregate((furthest, next) => next.position.z > furthest.position.z ? next : furthest).position;
        
        List<(GameObject prefab, Vector3 position)> GenerateIsland(
            List<(GameObject prefab, Vector3 position)> scattered)
        {
            var islandScatter = new List<(GameObject prefab, Vector3 position)>();
            var startZ = scattered.Any() ? FurthestCenter(scattered).z + _distanceBetweenIslands : _runUpLength*2;
            var startOnTrack = _track.GetProjectionSample(new Vector3(0,0, startZ)).location;
            var islandBounds = (innerBound: startOnTrack.x +_closerEdge, outerBound: startOnTrack.x +_outerEdge);
            var islandCount = 0;
            var mainAssets = _randomAssetGroup;
            var additionalAssets = _randomAssetGroup;
            additionalAssets = (mainAssets != additionalAssets) ? additionalAssets : null;
            
            islandScatter.Add(PlaceInitialAsset(mainAssets.ElementAt(GlobalRandom.RandomInt(0, mainAssets.Count())), startZ, islandBounds));            
            while(DecideToStopIsland(islandScatter, islandCount++))
            {
                var asset = mainAssets.ElementAt(GlobalRandom.RandomInt(0, mainAssets.Count()));
                var position = PlaceAtRandomAngleInfront(asset, islandBounds, islandScatter);
                islandScatter.Add((prefab: asset.Key, position: position));
            }             
            
            return islandScatter;
        }            
        
        bool DecideToStopIsland(List<(GameObject prefab, Vector3 position)> islandScattered,int islandCount)
        {
            var islandArea = islandScattered.Sum(placed => AssetArea(placed.prefab));
            var chance = (int)((_islandControllArea / islandArea)*100);
            return GlobalRandom.RandomIntInclusive(0, 100) < chance;            
        }    
        
        float AssetArea(GameObject prefab) => Mathf.PI + Mathf.Pow(AssetRadius(prefab), 2);
        
        (GameObject prefab, Vector3 position) PlaceInitialAsset(
            KeyValuePair<GameObject, float> asset, float offsetZ,
            (float innerBound, float outerBound) bounds)
        {
            var offsetFromTrack = (float)GlobalRandom.RandomDouble(bounds.innerBound, bounds.outerBound);
            var position = new Vector3(offsetFromTrack, _scatterHeight, offsetZ);
            return (prefab: asset.Key, position: position);
        }
        
        Vector3 PlaceAtRandomAngleInfront(
            KeyValuePair<GameObject, float> asset, (float innerBound, float outerBound) bounds,
            List<(GameObject prefab, Vector3 position)> islandScattered)
        {
            var previusAssetCenter = ConvertTo2D(islandScattered.Last().position);
            var minimalLength = (AssetRadius(islandScattered.Last().prefab) + asset.Value) * 0.8f;
            var length = (float)GlobalRandom.RandomDouble(minimalLength, minimalLength*1.8*_parameters.dencityCoefficient);
            var angle = (float)GlobalRandom.RandomDouble(_angleRange.min, _angleRange.max);
            var position2d = previusAssetCenter + new Vector2(length*Mathf.Cos(angle), length*Mathf.Sin(angle));
            position2d = MoveInsideBounds(position2d, bounds);
            var position = ConvertTo3D(position2d, _scatterHeight);
            while(IntersectWithAny(asset, position, islandScattered))
                position = NudgeFrontByRadius(asset, position);
            
            return position;
        }
        
        float AssetRadius(GameObject prefab) => _combinedAssetRadii[prefab];
        Vector2 MoveInsideBounds(Vector2 point, (float innerBound, float outerBound) bounds)
        {
            var correctedOffset = Mathf.Clamp(point.x, bounds.innerBound, bounds.outerBound);
            if(correctedOffset == point.x)
                return point;
                
            var newPoint = new Vector2(correctedOffset, point.y);
            newPoint.y += Vector2.Distance(newPoint, point);
            return newPoint;
        }
        
        bool IntersectWithAny(
            KeyValuePair<GameObject, float> asset, Vector3 position,
            List<(GameObject prefab, Vector3 position)> islandScattered)
                => islandScattered.Any(entry 
                    => Vector2.Distance(ConvertTo2D(entry.position), ConvertTo2D(position)) < AssetRadius(entry.prefab)+asset.Value);
                    
        Vector3 NudgeFrontByRadius(KeyValuePair<GameObject, float> asset, Vector3 position)
            => new Vector3(0, 0, asset.Value) + position;
        
        Vector2 ConvertTo2D(Vector3 point) => new Vector2(point.x, point.z);
        Vector3 ConvertTo3D(Vector2 point, float y) => new Vector3(point.x, y, point.y);
        
        
        Dictionary<GameObject, float> _combinedAssetRadii
            => _gproupsOfAssetsWithRadii.SelectMany(entry => entry).ToDictionary(x => x.Key, y => y.Value);
        Dictionary<GameObject, float> _randomAssetGroup
            => _gproupsOfAssetsWithRadii.ElementAt(GlobalRandom.RandomInt(0, _gproupsOfAssetsWithRadii.Count()));
            
        // List<(GameObject prefab, Vector3 po  sition)> ScatterAlongTheTrack(
        //         IEnumerable<Dictionary<GameObject, float>> groupsOfPrefabs, Spline track,
        //         (float dencityCoefficient, float width) parameters)
        // {Q
        //     var scattered = new List<(GameObject prefab, Vector3 position)>();
            
        //     var positionOnTrack = _runUpLength;
        //     var nextClumpChance = 20;
        //     var group = GlobalRandom.RandomInt(0, groupsOfPrefabs.Count());
            
        //     while(positionOnTrack < track.Length)
        //     {
        //         if((GlobalRandom.RandomIntInclusive(0, nextClumpChance) > nextClumpChance-1))
        //         {
        //             group = GlobalRandom.RandomInt(0, groupsOfPrefabs.Count());
        //             positionOnTrack += _runUpLength/2;
        //         }
                    
        //         var left = GlobalRandom.RandomBool();
        //         var assetGroup = groupsOfPrefabs.ToArray()[group].ToArray();
        //         var asset = assetGroup[GlobalRandom.RandomInt(0, assetGroup.Count())];
        //         var position = Place(asset.Value, track.GetSampleAtDistance(positionOnTrack).location, parameters.width, scattered, left);         
        //         positionOnTrack += asset.Value;
        //         scattered.Add((asset.Key, position));         
        //     }            
            
        //     return scattered;
        // }
        
        // Vector3 Place(float radius, Vector3 pointAtTrack, float maxWidth, List<(GameObject prefab, Vector3 position)> alreadyPlaced, bool left)
        // {
        //     var insideKeepOutWidth = maxWidth * 0.35f;
        //     var minDistanceFromTrack = radius + insideKeepOutWidth;
        //     var initialOffset = minDistanceFromTrack >= maxWidth ? 
        //         minDistanceFromTrack : 
        //         (float)GlobalRandom.RandomDouble(minDistanceFromTrack, maxWidth);    
                
        //     initialOffset = left ? -initialOffset : initialOffset;        
            
        //     var position = pointAtTrack + new Vector3(initialOffset, 0, 0);
        //     position.y = _scatterHeight;
        //     return position;
        // }
    }
}