using System;
using OS_2.Concepts;
using OS_2.Utils;

namespace OS_2.Modules
{
    public class MemoryManagementUnit
    {
        private readonly Memory _memory;
        
        private readonly PageTable _pageTable;

        public MemoryManagementUnit(Memory memory, PageTable pageTable)
        {
            _memory = memory;
            _pageTable = pageTable;
        }

        public int ConvertVirtualToReal(int virtualAddress)
        {
            int pageIndex = virtualAddress / Constants.PAGE_SIZE_IN_BYTES;
            int offset = virtualAddress % Constants.PAGE_SIZE_IN_BYTES;
            int realPageAddress = _pageTable[pageIndex];
            
            // if address is empty, we imply it's not in page table
            if (realPageAddress == 0)
            {
                throw new ArgumentException("Page not in table");
            }
            return realPageAddress + offset;
        }

        public int AccessMemory(int virtualAddress)
        {
            int realAddress = ConvertVirtualToReal(virtualAddress);
            return _memory[realAddress];
        }
    }
}