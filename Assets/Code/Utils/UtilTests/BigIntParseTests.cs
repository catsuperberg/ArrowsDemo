using ExtensionMethods;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
[RequiresPlayMode(false)]
public class BigIntParseTests
{
    [Test]    
    public void ParseToReadableTest()
    {
        Assert.That(new BigInteger(1).ParseToReadable(), Is.EqualTo("1"));
        Assert.That(new BigInteger(12).ParseToReadable(), Is.EqualTo("12"));
        Assert.That(new BigInteger(123).ParseToReadable(), Is.EqualTo("123"));
        
        Assert.That(new BigInteger(1000).ParseToReadable(), Is.EqualTo("1000"));
        Assert.That(new BigInteger(1234).ParseToReadable(), Is.EqualTo("1234"));
        Assert.That(new BigInteger(21234).ParseToReadable(), Is.EqualTo("21.23 k"));
        Assert.That(new BigInteger(321234).ParseToReadable(), Is.EqualTo("321.23 k"));
        
        Assert.That(new BigInteger(1000000).ParseToReadable(), Is.EqualTo("1.00 mil"));
        Assert.That(new BigInteger(4321234).ParseToReadable(), Is.EqualTo("4.32 mil"));
        Assert.That(new BigInteger(54321234).ParseToReadable(), Is.EqualTo("54.32 mil"));
        Assert.That(new BigInteger(654321234).ParseToReadable(), Is.EqualTo("654.32 mil"));
        
        
        Assert.That(new BigInteger(7654321234).ParseToReadable(), Is.EqualTo("7.65 bil"));
        Assert.That(new BigInteger(765432123400).ParseToReadable(), Is.EqualTo("765.43 bil"));
        Assert.That(new BigInteger(7654321234000).ParseToReadable(), Is.EqualTo("7.65 tril"));
        Assert.That(new BigInteger(765432123400000).ParseToReadable(), Is.EqualTo("765.43 tril"));
        Assert.That(new BigInteger(7654321234000000).ParseToReadable(), Is.EqualTo("7.65 quadr"));
        Assert.That(new BigInteger(765432123400000000).ParseToReadable(), Is.EqualTo("765.43 quadr"));
        
        Assert.That(new BigInteger(1234567890123456789).ParseToReadable(), Is.EqualTo("1.23 E18"));
        Assert.That(new BigInteger(12345678901234567890).ParseToReadable(), Is.EqualTo("1.23 E19"));
    }
}

// 1,000,000

