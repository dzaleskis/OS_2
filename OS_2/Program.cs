using System;
using System.Diagnostics;
using System.Threading;
using Monitor = OS_2.IO.Monitor;

namespace OS_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            var monitor = new Monitor();
            monitor.StartRunning();
            monitor.WriteTo(0, 65);
            monitor.WriteTo(1, 66);
            Thread.Sleep(1000);
            monitor.WriteTo(0, 80);
            monitor.WriteTo(1, 85);
            Thread.Sleep(1000);
            monitor.StopRunning();
        }
    }
}