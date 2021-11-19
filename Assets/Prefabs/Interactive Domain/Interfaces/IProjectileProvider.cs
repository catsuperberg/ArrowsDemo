using UnityEngine;
using System.Numerics;

namespace GamePlay
{    
    public interface IProjectileProvider
    {
        GameObject CreateArrows(BigInteger initialCount, float movementWidth);
    }
}