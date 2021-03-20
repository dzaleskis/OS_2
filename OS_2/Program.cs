using System;
using System.IO;
using System.Threading;
using OS_2.Machines;
using OS_2.Utils;

namespace OS_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter something...");
            var str = Console.ReadLine();
            File.WriteAllText(Constants.FLOPPY_FILENAME, str);
            var machine = new Machine();
            Thread.Sleep(200);
            for (int i = 0; i < str.Length; i++)
            {
                machine.ExecuteCycle();
                Thread.Sleep(100);
            }
        }
    }
}