using SplineMesh;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public interface ISplineTrackProvider
    {
        GameObject GetRandomizedTrack(float length, GameObject splineMeshPrefab);
        Task<Spline> GetRandomizedTrackAsync(float length, GameObject splineMeshPrefab);
    }
}