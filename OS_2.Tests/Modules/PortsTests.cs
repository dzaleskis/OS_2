using System;
using Moq;
using NUnit.Framework;
using OS_2.Concepts;
using OS_2.Modules;

namespace OS_2.Tests.Modules
{
    public class PortsTests
    {
        private Ports Ports = new Ports();
        private Mock<IPortDevice> portDeviceMock = new Mock<IPortDevice>();
        
        [SetUp]
        public void Setup()
        {
            portDeviceMock.Setup(m => m.GetRequiredPorts()).Returns(10);
        }
        
        [Test]
        public void ForwardsDataToDeviceCorrectly()
        {
            Ports.AllocateDevice(portDeviceMock.Object);
            Ports.WriteToPort(5, Byte.MaxValue);
            portDeviceMock.Verify(m => m.WriteTo(5, Byte.MaxValue));
        }
        
        [Test]
        public void WritesToMultipleDevicesCorrectly()
        {
            Ports.AllocateDevice(portDeviceMock.Object);
            Ports.AllocateDevice(portDeviceMock.Object);
            Ports.WriteToPort(5, Byte.MaxValue);
            Ports.WriteToPort(16, Byte.MinValue);
            portDeviceMock.Verify(m => m.WriteTo(5, Byte.MaxValue));
            portDeviceMock.Verify(m => m.WriteTo(6, Byte.MinValue));
        }
        
        [Test]
        public void ReadsFromDeviceCorrectly()
        {
            Ports.AllocateDevice(portDeviceMock.Object);
            Ports.ReadFromPort(8);
            portDeviceMock.Verify(m => m.ReadFrom(8));
        }
        
        [Test]
        public void ReadsFromMultipleDevicesCorrectly()
        {
            Ports.AllocateDevice(portDeviceMock.Object);
            Ports.AllocateDevice(portDeviceMock.Object);
            Ports.ReadFromPort(5);
            Ports.ReadFromPort(16);
            portDeviceMock.Verify(m => m.ReadFrom(5));
            portDeviceMock.Verify(m => m.ReadFrom(6));
        }
        
        [Test]
        public void ThrowsIfNoDeviceFound()
        {
            Assert.Throws<InvalidOperationException>(() => Ports.WriteToPort(55, Byte.MaxValue));
        }
        
        [TearDown]
        public void Cleanup()
        {
            Ports.Reset();
        }
    }
}