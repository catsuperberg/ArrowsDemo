using System.Numerics;

namespace ExtensionMethods
{
    public static class BigIntegerOperations
    {    
            public static BigInteger multiplyByFraction(this BigInteger value, double multiplier)
            {
                int[] bits = decimal.GetBits((decimal)multiplier);
                BigInteger numerator = (1 - ((bits[3] >> 30) & 2)) *
                                    unchecked(((BigInteger)(uint)bits[2] << 64) |
                                                ((BigInteger)(uint)bits[1] << 32) |
                                                (BigInteger)(uint)bits[0]);
                BigInteger denominator = BigInteger.Pow(10, (bits[3] >> 16) & 0xff);
                return value * numerator/denominator;
            }        
    }    
}