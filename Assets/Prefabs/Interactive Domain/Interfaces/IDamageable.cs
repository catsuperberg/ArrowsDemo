using System.Numerics;

namespace GamePlay
{    
    public interface IDamageable
    {
        public BigInteger DamagePoints {get;}
        public void Damage(BigInteger value);
    }
}