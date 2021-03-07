using System;
using NUnit.Framework;
using OS_2.Concepts;
using OS_2.Machines;
using OS_2.Modules;

namespace OS_2.Tests.Modules
{
    public class MemoryManagementUnitTests
    {
        private Memory memory;
        private PageTable pageTable;
        private MemoryManagementUnit unit;
        
        [SetUp]
        public void Setup()
        {
            memory = new Memory();
            pageTable = new PageTable();
            unit = new MemoryManagementUnit(memory, pageTable);
        }
        
        [Test]
        public void ConvertsAddressCorrectly()
        {
            pageTable[0] = 22; // real address of page is 22
            var result = unit.ConvertVirtualToReal(30); // access virtual address 30
            Assert.That(result == 30 + 22);
        }

        [Test]
        public void AccessesMemoryCorrectly1()
        {
            memory.Clear();
            memory[512] = byte.MaxValue;
            pageTable[1] = 512;
            var result = unit.AccessMemory(512);
            Assert.That(result == byte.MaxValue);
        }
        
        [Test]
        public void AccessesMemoryCorrectly2()
        {
            memory.Clear();
            memory[513] = short.MaxValue; // only FF will be read
            pageTable[1] = 512;
            var result = unit.AccessMemory(512);
            Assert.That(result == -256); // FF00 should give -256 in two's complement
        }
        
        [Test]
        public void ThrowsOnUnmappedPage()
        {
            pageTable[1] = 0;
            Assert.Throws<ArgumentException>(() => unit.AccessMemory(512));
        }
    }
}