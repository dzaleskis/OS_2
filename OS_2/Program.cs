using System;
using System.Threading;
using Monitor = OS_2.IO.Monitor;

namespace OS_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var monitor = new Monitor();
            monitor.WriteTo(0, 65);
            monitor.WriteTo(1, 66);
            Thread.Sleep(1000);
            monitor.WriteTo(0, 80);
            monitor.WriteTo(1, 85);
            Thread.Sleep(1000);
        }
    }
}