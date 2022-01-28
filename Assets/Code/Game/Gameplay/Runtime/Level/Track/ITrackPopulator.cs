using Game.Gameplay.Runtime.OperationSequence.Operation;
using SplineMesh;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay.Runtime.Level.Track
{
    public interface ITrackPopulator
    {
        public GameObject PlaceGates(GameObject gatePrefab, Spline track, OperationPairsSequence sequence);
        public GameObject SpreadObjects(List<GameObject> prefabsToSpread, int dencityCoefficient);
    }
}