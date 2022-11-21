using System;
using System.Numerics;

namespace Game.GameDesign
{
    public struct UpgradeContainer : IComparable<UpgradeContainer>
    {
        public string Name;
        public int Level {get; private set;}
        // public int Level {get => Level; set {Level = value; Price = _getPrice(Name, Level);}}
        public void IncrementLevel() {Level++; Price = _getPrice(Name, Level);}
        // public BigInteger Price {get => _getPrice(Name, Level);}
        public BigInteger Price {get; private set;}
        
        Func<string, int, BigInteger> _getPrice;
        
        public UpgradeContainer(string name, int level, Func<string, int, BigInteger> priceFunction)
        {
            Name = name;
            Level = level;
            _getPrice = priceFunction;
            Price = _getPrice(Name, Level);
        }
                
            
        public int CompareTo(UpgradeContainer incomingobject)
            => this.Price.CompareTo(incomingobject.Price);
    }
}