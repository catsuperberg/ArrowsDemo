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
        public Task<GameObject> SpreadObjectsAsync(List<GameObject> prefabsToSpread, int dencityCoefficient, IInstatiator assetInstatiator);
    }
}