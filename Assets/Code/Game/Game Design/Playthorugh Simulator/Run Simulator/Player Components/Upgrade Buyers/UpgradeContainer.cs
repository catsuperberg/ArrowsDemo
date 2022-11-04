using System;
using System.Numerics;

namespace Game.GameDesign
{
    public class UpgradeContainer
    {
        public string Name;
        public int Level;
        public BigInteger Price {get => _getPrice(Name, Level);}
        
        Func<string, int, BigInteger> _getPrice;
        
        public UpgradeContainer(string name, int level, Func<string, int, BigInteger> priceFunction)
        {
            Name = name;
            Level = level;
            _getPrice = priceFunction;
        }
    }
}