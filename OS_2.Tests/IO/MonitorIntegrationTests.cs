using NUnit.Framework;
using OS_2.IO;
using OS_2.Utils;

namespace OS_2.Tests.IO
{
    public class MonitorIntegrationTests
    {
        private Monitor monitor;

        [SetUp]
        public void Setup()
        {
            monitor = new Monitor();
        }
        
        [Test]
        public void ComputesFrameCorrectly()
        {
            var content = "labadiena";
            var contentBytes = System.Text.Encoding.ASCII.GetBytes(content);

            for (int i = 0; i < contentBytes.Length; i++)
            {
                monitor.WriteTo(i, contentBytes[i]);
            }

            StringAssert.StartsWith(content, monitor.ComputeFrame());
        }

        [TearDown]
        public void Cleanup()
        {
            monitor.Dispose();
        }
    }
}