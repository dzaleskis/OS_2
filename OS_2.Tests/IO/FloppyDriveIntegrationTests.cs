using System;
using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using OS_2.IO;
using OS_2.Modules;

namespace OS_2.Tests.IO
{
    public class FloppyDriveIntegrationTests
    {
        private FloppyDrive floppyDrive;
        private string filename = "test.txt";
        private string content = "Hello World!";
        private Mock<InterruptController> controllerMock;
        
        [SetUp]
        public void Setup()
        {
            File.WriteAllText(filename, content);
            controllerMock = new Mock<InterruptController>();
            floppyDrive = new FloppyDrive(filename, controllerMock.Object);
        }
        
        [Test]
        public void SignalsErrorIfFifoEmpty()
        {
            floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO);
            
            Assert.That((byte) FloppyError.NothingToRead == floppyDrive.ReadFrom((int) FloppyRegister.Error));
        }
        
        [Test]
        public void SignalsErrorOnWriteToFifo()
        {
            floppyDrive.WriteTo((int) FloppyRegister.DataFIFO, 5);
            
            Assert.That((byte) FloppyError.IncorrectAccess == floppyDrive.ReadFrom((int) FloppyRegister.Error));
        }

        [Test]
        public void ReadsFromDiskCorrectly()
        {
            floppyDrive.WriteTo((int) FloppyRegister.LBA, 0);
            floppyDrive.WriteTo((int) FloppyRegister.Control, (int) FloppyControl.ReadData);
            floppyDrive.StartRunning();
            Thread.Sleep(200);
            
            Assert.That('H' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            Assert.That('e' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            Assert.That('l' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            Assert.That('l' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            Assert.That('o' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            
        }

        [TearDown]
        public void Cleanup()
        {
            File.Delete(filename);
            floppyDrive.StopRunning();
        }

    }
}