using System;
using NUnit.Framework;
using OS_2.Machines;

namespace OS_2.Tests
{
    public class Tests
    {
        private VirtualMachine _vm;
        
        [SetUp]
        public void Setup()
        {
            _vm = new VirtualMachine();;
        }

        [Test]
        public void ExecuteThrowsNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => _vm.ExecuteCycle());
        }
    }
}