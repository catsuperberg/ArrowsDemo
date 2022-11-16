using ExtensionMethods;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
[RequiresPlayMode(false)]
public class BigIntFastFractionalTests
{
    [Test]    
    public void DisplayResults()
    {
        var value = new BigInteger(1000);
        Debug.Log($"{value} * 10000.5 = {value.multiplyByFractionFast(10000.5)}");
        Debug.Log($"{value} * 100.8 = {value.multiplyByFractionFast(100.8)}");
        Debug.Log($"{value} * 1.456456 = {value.multiplyByFractionFast(1.456456)}");
        Debug.Log($"{value} * 0.2 = {value.multiplyByFractionFast(0.2)}");
        Debug.Log($"{value} * 0.085 = {value.multiplyByFractionFast(0.085)}");
        Debug.Log($"{value} * 0.0085 = {value.multiplyByFractionFast(0.0085)}");
        Debug.Log($"{value} * 0.000085 = {value.multiplyByFractionFast(0.000085)}");
    }
}
