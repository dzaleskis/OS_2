using System;
using OS_2.Concepts;
using OS_2.Utils;

namespace OS_2.Modules
{
    public class MemoryManagementUnit
    {
        private readonly Memory _memory;
        public int PT { get; set; }
        public int CR1 { get; set; }
        
        public MemoryManagementUnit(Memory memory)
        {
            _memory = memory;
        }

        public int ConvertVirtualToReal(int virtualAddress)
        {
            int pageIndex = virtualAddress / Constants.PAGE_SIZE_IN_BYTES;
            int offset = virtualAddress % Constants.PAGE_SIZE_IN_BYTES;
            int realPageAddress = _memory[PT + pageIndex * Constants.WORD_LENGTH];
            
            // if address is empty, we imply it's not in page table
            if (realPageAddress == 0)
            {
                // need to specify which virtual address failed translation
                CR1 = virtualAddress;
                throw new ArgumentException("Page not in table");
            }
            return realPageAddress + offset;
        }
    }
}