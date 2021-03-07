using System;
using OS_2.Concepts;

namespace OS_2.Modules
{
    public class MemoryManagementUnit
    {
        private readonly Memory _memory;
        
        private readonly PageTable _pageTable;

        private const ushort PAGE_SIZE_IN_BYTES = 512;

        public MemoryManagementUnit(Memory memory, PageTable pageTable)
        {
            _memory = memory;
            _pageTable = pageTable;
        }

        public ushort ConvertVirtualToReal(ushort virtualAddress)
        {
            int pageIndex = virtualAddress / PAGE_SIZE_IN_BYTES;
            ushort offset = (ushort) (virtualAddress % PAGE_SIZE_IN_BYTES);
            ushort realPageAddress = _pageTable[pageIndex];
            
            // if address is empty, we imply it's not in page table
            if (realPageAddress == 0)
            {
                throw new ArgumentException("Page not in table");
            }
            return (ushort) (realPageAddress + offset);
        }

        public short AccessMemory(ushort virtualAddress)
        {
            ushort realAddress = ConvertVirtualToReal(virtualAddress);
            return _memory[realAddress];
        }
    }
}