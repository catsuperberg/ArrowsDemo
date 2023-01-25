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
        public readonly BigInteger BaseValue;
        [JsonProperty]
        public readonly double BasePower;
        [JsonProperty]
        public readonly double BaseIncrement;
        [JsonProperty]
        public readonly double IncrementPower;

        [JsonConstructor]
        public UpgradePriceFormula(BigInteger baseValue, double basePower, double baseIncrement, double incrementPower)
        {
            BaseValue = baseValue;
            BasePower = basePower;
            BaseIncrement = baseIncrement;
            IncrementPower = incrementPower;
        }
        
        public UpgradePriceFormula(
            UpgradePriceFormula prototype, BigInteger? baseValue = null, double? basePower = null, 
            double? baseIncrement = null, double? incrementPower = null)
        {
            BaseValue = baseValue ?? prototype.BaseValue;
            BasePower = basePower ?? prototype.BasePower;
            BaseIncrement = baseIncrement ?? prototype.BaseIncrement;
            IncrementPower = incrementPower ?? prototype.IncrementPower;
        }

        public BigInteger Evaluate(int level)
        {
            var power = BasePower + (BaseIncrement * Math.Pow(level, IncrementPower));
            return BaseValue.PowFractional(power);
        }
    }
}