using System;
using NUnit.Framework;
using OS_2.Utils;

namespace OS_2.Tests.Utils
{
    public class BitUtilsTests
    {
        [Test]
        public void ConvertsShortToUshortCorrectly()
        {
            short s = -1;
            // -1 in signed two's complement is 0xFFFF
            Assert.That(s.ToUshort() == ushort.MaxValue);
        }
        
        [Test]
        public void ConvertsIntToUshortCorrectly()
        {
            int i = -1;
            // -1 in signed two's complement is 0xFFFF
            Assert.That(i.ToUshort() == ushort.MaxValue);
        }
        
        [Test]
        public void ConvertsIntToShortCorrectly()
        {
            int i = short.MinValue;
            Assert.That(i.ToShort() == short.MinValue);
        }
        
        [Test]
        public void ThrowsIfDoesntFit()
        {
            int i = short.MinValue - 1;
            Assert.Throws<OverflowException>(() => i.ToShort());
        }
        
        [Test]
        public void ChecksLastBitCorrectly()
        {
            short s = -1;
            // -1 in signed two's complement is 0xFFFF
            Assert.That(s.IsLastBitSet());

            ushort u = ushort.MaxValue;
            // max value is also 0xFFFF
            Assert.That(u.IsLastBitSet());
        }
    }
}