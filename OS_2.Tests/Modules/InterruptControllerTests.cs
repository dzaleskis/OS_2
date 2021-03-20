using Moq;
using NUnit.Framework;
using OS_2.Concepts;
using OS_2.Modules;

namespace OS_2.Tests.Modules
{
    public class InterruptControllerTests
    {
        private InterruptController controller;
        private Mock<InterruptLine> deviceMock;

        [SetUp]
        public void Setup()
        {
            controller = new InterruptController();
            deviceMock = new Mock<InterruptLine>(controller);
        }
        
        [Test]
        public void ReturnsZeroIfNoRequestsMade()
        {
            Assert.That(controller.IRQ == 0);
        }
        
        [Test]
        public void HandlesInterruptCorrectly()
        {
            controller.HandleInterruptRequest(deviceMock.Object);
            Assert.That(controller.IRQ == 1);
        }
        
        [Test]
        public void HandlesMultipleInterruptsCorrectly()
        {
            var deviceMock2 = new Mock<InterruptLine>(controller);
            controller.HandleInterruptRequest(deviceMock.Object);
            controller.HandleInterruptRequest(deviceMock2.Object);
            Assert.That(controller.IRQ == 3);
        }
    }
}