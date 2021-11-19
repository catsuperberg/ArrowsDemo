using System.Numerics;
using UnityEngine;

namespace GamePlay
{    
    public interface IProjectileObject
    {
        public BigInteger Count {get;}
        public void Initialize(BigInteger initialCount, float movementWidth);
    }
}