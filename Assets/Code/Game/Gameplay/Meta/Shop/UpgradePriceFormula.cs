using System;
using System.Numerics;
using ExtensionMethods;
using Newtonsoft.Json;

namespace Game.Gameplay.Meta.Shop
{
    [Serializable]
    public class UpgradePriceFormula
    {
        [JsonProperty]
        BigInteger _baseValue;
        [JsonProperty]
        double _basePower;
        [JsonProperty]
        double _baseIncrement;
        [JsonProperty]
        double _IncrementPower;

        public UpgradePriceFormula(BigInteger baseValue, double basePower, double baseIncrement, double incrementPower)
        {
            _baseValue = baseValue;
            _basePower = basePower;
            _baseIncrement = baseIncrement;
            _IncrementPower = incrementPower;
        }

        public BigInteger Evaluate(int level)
        {
            var power = _basePower + (0.013f * Math.Pow(level, 1.2f));
            return _baseValue.PowFractional(power);
        }
    }
}