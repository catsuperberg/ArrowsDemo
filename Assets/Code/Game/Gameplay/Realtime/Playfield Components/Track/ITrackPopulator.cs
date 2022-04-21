using Game.Gameplay.Realtime.OperationSequence.Operation;
using SplineMesh;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public interface ITrackPopulator
    {
        public GameObject PlaceGates(GameObject gatePrefab, Spline track, OperationPairsSequence sequence);
        public Task<GameObject> PlaceGatesAsync(GameObject gatePrefab, Spline track, OperationPairsSequence sequence);
        public GameObject SpreadObjects(List<GameObject> prefabsToSpread, int dencityCoefficient);
    }
}