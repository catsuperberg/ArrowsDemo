using Game.Gameplay.Realtime.GeneralUseInterfaces;
using System.Numerics;

namespace Game.Gameplay.Realtime.PlayfildComponents.Target
{
    public interface ITargetGroup : IDamageable
    {        
        public BigInteger Count {get;}
    }
}