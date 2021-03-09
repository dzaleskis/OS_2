using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using OS_2.IO;

namespace OS_2.Tests.IO
{
    public class FloppyDriveIntegrationTests
    {
        private FloppyDrive floppyDrive;
        [SetUp]
        public void Setup()
        {
            var filename = "test.txt";
            var content = "Hello World!";
            File.WriteAllText(filename, content);
            floppyDrive = new FloppyDrive(filename);
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
            Thread.Sleep(200);
            
            Assert.That('H' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            Assert.That('e' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            Assert.That('l' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            Assert.That('l' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
            Assert.That('o' == Convert.ToChar(floppyDrive.ReadFrom((int) FloppyRegister.DataFIFO)));
        }
    }
}