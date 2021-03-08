using System;
using OS_2.Utils;

namespace OS_2.Modules
{
    public class Memory
    {

        private byte[] mem = new byte[Constants.TOTAL_MEMORY_SIZE];
        
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

        public void Reset()
        {
            mem = new byte[Constants.TOTAL_MEMORY_SIZE];
        }
    }
}