using AssetScripts.Instantiation;
using SplineMesh;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Realtime.PlayfieldComponents.Track
{
    public interface ISplineTrackProvider
    {
        Task<Spline> GetRandomizedTrackAsync(float length, GameObject splineMeshPrefab, IInstatiator assetInstatiator);
    }
}