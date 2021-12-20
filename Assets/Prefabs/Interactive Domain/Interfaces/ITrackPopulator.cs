using Sequence;
using SplineMesh;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    namespace Track
    {
        public interface ITrackPopulator
        {
            public GameObject PlaceGates(GameObject gatePrefab, Spline track, OperationPairsSequence sequence);
            public GameObject SpreadObjects(List<GameObject> prefabsToSpread, int dencityCoefficient);
        }
    }
}