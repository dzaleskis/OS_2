using System;
using System.Linq;
using OS_2.Utils;

namespace OS_2.Modules
{
    public class Memory
    {
        private byte[] mem = new byte[Constants.TOTAL_MEMORY_SIZE];
        
        public int this[int index]
        {
            get
            {
                var firstByte = mem[index];
                var secondByte = mem[index + 1];
                return BitConverter.ToInt16(new byte[] {firstByte, secondByte});
            }

            set
            {
                var bytes = BitConverter.GetBytes((short)value);
                bytes.CopyTo(mem, index);
            }
        }

        public void Reset()
        {
            mem = new byte[Constants.TOTAL_MEMORY_SIZE];
        }
    }
}