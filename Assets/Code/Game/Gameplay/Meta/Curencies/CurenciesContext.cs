using DataManagement;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Game.Gameplay.Meta.Curencies
{
    public class CurenciesContext : Configurable
    {
        [StoredField]
        public BigInteger CommonCoins {get; private set;} = 0;
        [StoredField]
        public BigInteger SkinTokens {get; private set;} = 0;
        [StoredField]
        public BigInteger LifetimeSpending {get; private set;} = 0;
        
        public CurenciesContext()
        {            
        }
        
        public CurenciesContext(IRegistryIngester registry)
        {
            registry.Register(this, true, true);            
        }    
        
        internal override void SetFieldValue(string fieldName, string fieldValue)
        {
            switch(fieldName)
            {
                case nameof(CommonCoins):                 
                    CommonCoins = BigInteger.Parse(fieldValue);
                    break;
                case nameof(SkinTokens):
                    SkinTokens = BigInteger.Parse(fieldValue);
                    break;
                case nameof(LifetimeSpending):
                    LifetimeSpending = BigInteger.Parse(fieldValue);
                    break;
                default:
                    throw new MissingFieldException("No such field in this class: " + fieldName + " Class name: " + this.GetType().Name);
            }
        }
    }
}