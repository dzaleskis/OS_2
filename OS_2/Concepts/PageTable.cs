using System;

namespace OS_2.Concepts
{
    public class PageTable
    {
        // default value of int is 0, so if address is not in table, we return 0
        private ushort[] pages = new ushort[128];
        
        public ushort this[int index]
        {
            get
            {
                if (index < 0 || index >= pages.Length)
                    throw new IndexOutOfRangeException("Index out of range");

                return pages[index];
            }

            set
            {
                if (index < 0 ||  index >= pages.Length)
                    throw new IndexOutOfRangeException("Index out of range");

                pages[index] = value;
            }
        }
    }
}