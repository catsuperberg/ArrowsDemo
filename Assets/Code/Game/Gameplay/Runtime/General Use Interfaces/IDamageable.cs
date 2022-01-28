using System.Numerics;

namespace Game.Gameplay.Runtime.GeneralUseInterfaces
{    
    public interface IDamageableWithTransforms : IDamageable, ITransformContainer{}
    
    public interface IDamageable
    {
        public BigInteger DamagePoints {get;}
        public void Damage(BigInteger value);
    }
}