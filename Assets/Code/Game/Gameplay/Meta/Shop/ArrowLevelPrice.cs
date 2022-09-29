using ExtensionMethods;
using MathNet.Numerics;
using System;
using System.Numerics;

namespace Game.Gameplay.Meta.Shop
{
    public class ArrowLevelPrice : IItemPriceCalculator
    {
        BigInteger _levelOnePrice = 200;
        BigInteger _growthSpeedCoefficient = 120;
        
        double[] _polynomialCoeficients = new double[]{
            4.87295414e-04, -3.37039995e-01,  9.93879179e+01, -1.62955265e+04,
            1.62457929e+06, -1.01192456e+08,  3.89668132e+09, -8.85438938e+10,
            1.07737418e+12, -5.77613014e+12,  8.52221277e+12};
        
        public BigInteger GetPrice(PricingContext context)
        {
            var itemLevel = context.ItemLevel;
            // var initialPrice = _levelOnePrice;
            // var power = 1.1 + (0.013 * Math.Pow(itemLevel, 1.2));
            // var price = initialPrice.PowFractional(power);
            // Array.Reverse(_polynomialCoeficients);
            // var pricePolynomial = new Polynomial(_polynomialCoeficients);
            // var newPrice = new BigInteger(pricePolynomial.Evaluate(itemLevel));
            var newPrice = new BigInteger(2.85*Math.Exp(0.219*itemLevel)+360);
            var price = newPrice;
            return price;
        }
    }
}