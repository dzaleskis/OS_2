using System;

namespace OS_2.Utils
{
    public static class BitUtils
    {
        public static bool IsBitSet(this short s, int pos)
        {
            return (s & (1 << pos)) != 0;
        }

        public static bool IsLastBitSet(this short s)
        {
            return IsBitSet(s, 15);
        }
        
        public static bool IsBitSet(this ushort u, int pos)
        {
            return (u & (1 << pos)) != 0;
        }

        public static bool IsLastBitSet(this ushort u)
        {
            return IsBitSet(u, 15);
        }

        public static ushort ToUshort(this short s)
        {
            return unchecked((ushort) s);
        }
        
        public static ushort ToUshort(this int i)
        {
            // we don't want sketchy conversions, only those that actually can fit
            if (i < short.MinValue || i > ushort.MaxValue)
            {
                throw new OverflowException();
            }
            return unchecked((ushort) i);
        }
        
        public static short ToShort(this int i)
        {
            return Convert.ToInt16(i);
        }
        
    }
}