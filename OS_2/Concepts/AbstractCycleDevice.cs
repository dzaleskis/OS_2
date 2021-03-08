using System;
using System.Threading;

namespace OS_2.Concepts
{
    public abstract class AbstractCycleDevice
    {
        protected int Timeout { get; set; } = 500;
        
        protected Timer Timer;

        protected AbstractCycleDevice()
        {
            UpdateTimer();
        }

        protected void UpdateTimer()
        {
            Timer = new Timer(DoCycle, null, 0, Timeout);
        }
        
        protected abstract void DoCycle(Object stateInfo);
    }
}