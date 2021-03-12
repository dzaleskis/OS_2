using System;
using System.Collections.Generic;
using System.Linq;
using OS_2.Concepts;

namespace OS_2.Modules
{
    public class InterruptController: IInterruptController
    {
        private byte _irq = 0;

        private readonly List<InterruptLine> _interruptLines = new List<InterruptLine>();

        public void RegisterInterruptLine(InterruptLine line)
        {
            _interruptLines.Add(line);
        }
        
        public void HandleInterruptRequest(InterruptLine line)
        {
            var index = _interruptLines.IndexOf(line);
            var bitToMark = (byte) (1 << index);
            _irq |= bitToMark;
        }

        public byte ReadInterruptRequests()
        {
            var toReturn = _irq;
            _irq = 0;
            return toReturn;
        }

        private static byte Pow(int x, int pow)
        {
            return (byte) Math.Pow(x, pow);
        }
    }
}