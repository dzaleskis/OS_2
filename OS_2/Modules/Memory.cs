using System;

namespace OS_2.Modules
{
    public class Memory
    {
        private const int MEMORY_SIZE = 65536;
        
        private byte[] mem = new byte[MEMORY_SIZE];
        
        public short this[ushort index]
        {
            get
            {
                var firstByte = mem[index];
                var secondByte = mem[index + 1];
                return BitConverter.ToInt16(new byte[] {firstByte, secondByte});
            }

            set
            {
                var bytes = BitConverter.GetBytes(value);
                bytes.CopyTo(mem, index);
            }
        }

        public void Clear()
        {
            mem = new byte[MEMORY_SIZE];
        }
    }
}