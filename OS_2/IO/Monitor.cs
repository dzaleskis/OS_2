using System;
using OS_2.Concepts;
using OS_2.Utils;

namespace OS_2.IO
{
    public class Monitor: AbstractCycleDevice, IPortDevice
    {
        private const int totalMemory = Constants.MONITOR_VERTICAL * Constants.MONITOR_HORIZONTAL;
        private byte[] memory = new byte[totalMemory];
        
        public int GetRequiredPorts()
        {
            return totalMemory;
        }

        public void WriteTo(int accessedIndex, byte value)
        {
            memory[accessedIndex] = value;
        }

        public byte ReadFrom(int accessedIndex)
        {
            return memory[accessedIndex];
        }

        public string ComputeFrame()
        {
            return System.Text.Encoding.ASCII.GetString(memory, 0, memory.Length);
        }

        private void Render()
        {
            Console.WindowWidth = Constants.MONITOR_HORIZONTAL;
            Console.WindowHeight = Constants.MONITOR_VERTICAL;
            Console.Clear();
            Console.Write(ComputeFrame());
        }

        public override void DoCycle()
        {
            Render();
        }
    }
}