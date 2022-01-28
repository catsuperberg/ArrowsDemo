using Game.Gameplay.Runtime.GeneralUseInterfaces;
using System.Numerics;

namespace Game.Gameplay.Runtime.Level.Target
{
    public interface ITargetGroup : IDamageable
    {        
        public BigInteger Count {get;}
    }
}