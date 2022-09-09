using AssetScripts.Instantiation;
using Game.Gameplay.Realtime.OperationSequence.Operation;
using SplineMesh;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public interface ITrackPopulator
    {
        public Task<GameObject> PlaceGatesAsync(GameObject gatePrefab, Spline track, OperationPairsSequence sequence, IInstatiator assetInstatiator);
        public Task<GameObject> SpreadBackgroundScatterAsync(
            IEnumerable<IEnumerable<GameObject>> GroupsOfPrefabsToSpread, Spline track, 
            (float dencityCoefficient, float width) parameters, IInstatiator assetInstatiator);
    }
}