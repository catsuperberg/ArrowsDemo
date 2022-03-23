using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public interface ISplineTrackProvider
    {
        GameObject GetRandomizedTrack(float length, GameObject splineMeshPrefab);
    }
}