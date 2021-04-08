using System;
using NUnit.Framework;
using OS_2.Concepts;
using OS_2.Exceptions;
using OS_2.Modules;

namespace OS_2.Tests.Modules
{
    public class MemoryManagementUnitTests
    {
        private Memory memory;
        private MemoryManagementUnit unit;
        private const int PAGE_TABLE_ADDRESS = 1024;
        
        [SetUp]
        public void Setup()
        {
            memory = new Memory();
            unit = new MemoryManagementUnit(memory);
            unit.PT = PAGE_TABLE_ADDRESS;
        }
        
        [Test]
        public void ConvertsAddressCorrectly()
        {
            memory[PAGE_TABLE_ADDRESS] = 22; // real address of page is 22
            var result = unit.ConvertVirtualToReal(30); // access virtual address 30
            Assert.That(result == 30 + 22);
        }

        [Test]
        public void AccessesMemoryCorrectly1()
        {
            memory[512] = byte.MaxValue;
            memory[PAGE_TABLE_ADDRESS + 2] = 512;
            var realAddress = unit.ConvertVirtualToReal(512);
            Assert.That(memory[realAddress] == byte.MaxValue);
        }
        
        [Test]
        public void AccessesMemoryCorrectly2()
        {
            memory[513] = short.MaxValue; // only FF should be read
            memory[PAGE_TABLE_ADDRESS + 2] = 512;
            var realAddress = unit.ConvertVirtualToReal(512);
            Assert.That(memory[realAddress] == -256); // FF00 should give -256 in two's complement
        }
        
        [Test]
        public void ThrowsOnlyOnUnmappedPage()
        {
            memory[PAGE_TABLE_ADDRESS] = 5;
            Assert.DoesNotThrow(() => unit.ConvertVirtualToReal(50));
            
            memory[PAGE_TABLE_ADDRESS + 2] = 0;
            Assert.Throws<PageException>(() => unit.ConvertVirtualToReal(512));
        }
        
        [TearDown]
        public void Cleanup()
        {
            memory.Reset();
        }
    }
}