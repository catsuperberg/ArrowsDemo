using Sequence;
using SplineMesh;
using UnityEngine;

namespace Level
{
    namespace Track
    {
        public interface ISplineTrackProvider
        {
            GameObject GetRandomizedTrack(float length, GameObject splineMeshPrefab);
        }
    }
}
