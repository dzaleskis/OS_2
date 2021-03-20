using System;
using System.Collections.Generic;
using System.Linq;
using OS_2.Concepts;

namespace OS_2.Modules
{
    public class InterruptController: IInterruptController
    {
        public byte IRQ { get; set; }

        private readonly List<InterruptLine> _interruptLines = new List<InterruptLine>();

        public void RegisterInterruptLine(InterruptLine line)
        {
            _interruptLines.Add(line);
        }
        
        public void HandleInterruptRequest(InterruptLine line)
        {
            var index = _interruptLines.IndexOf(line);
            var bitToMark = (byte) (1 << index);
            IRQ |= bitToMark;
        }
    }
}