using System.Threading;
using Moq;
using NUnit.Framework;
using OS_2.Concepts;
using OS_2.Modules;
using Timer = OS_2.Modules.Timer;

namespace OS_2.Tests.Modules
{
    public class TimerTests
    {
        private Timer _timer;
        private Mock<IInterruptController> controllerMock;

        [SetUp]
        public void Setup()
        {
            controllerMock = new Mock<IInterruptController>();
            _timer = new Timer(controllerMock.Object);
            _timer.StartRunning();
        }
        
        [Test]
        public void DoesNotTriggerInterruptsByDefault()
        {
            Thread.Sleep(150);
            controllerMock.Verify(m => m.HandleInterruptRequest(_timer._interruptLine), Times.Never());
        }
        
        [Test]
        public void TriggersInterruptsIfEnabled()
        {
            _timer.WriteTo((int)TimerRegisters.EnableInterrupts, 1);
            Thread.Sleep(150);
            controllerMock.Verify(m => m.HandleInterruptRequest(_timer._interruptLine), Times.AtLeastOnce());
        }

        [TearDown]
        public void Cleanup()
        {
            _timer.StopRunning();
        }
    }
}