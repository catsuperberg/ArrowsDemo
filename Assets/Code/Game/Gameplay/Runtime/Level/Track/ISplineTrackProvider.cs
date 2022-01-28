using UnityEngine;

namespace Game.Gameplay.Runtime.Level.Track
{
    public interface ISplineTrackProvider
    {
        GameObject GetRandomizedTrack(float length, GameObject splineMeshPrefab);
    }
}