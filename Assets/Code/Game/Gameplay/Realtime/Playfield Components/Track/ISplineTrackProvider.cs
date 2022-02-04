using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfildComponents.Track
{
    public interface ISplineTrackProvider
    {
        GameObject GetRandomizedTrack(float length, GameObject splineMeshPrefab);
    }
}