using System.Numerics;
using GamePlay;

namespace Level
{
    namespace Target
    {
        public interface ITargetGroup : IDamageable
        {        
            public BigInteger Count {get;}
        }
    }
}