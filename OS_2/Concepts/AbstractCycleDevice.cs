using System;
using System.Threading;

namespace OS_2.Concepts
{
    public abstract class AbstractCycleDevice
    {
        protected int Timeout { get; set; } = 100;
        
        protected Timer Timer;

        protected AbstractCycleDevice()
        {
            Timer = new Timer(DoCycle, null, 0, Timeout);
        }

        protected void UpdateTimer()
        {
            
            Timer.Change(0, Timeout);
        }
        
        protected abstract void DoCycle(Object stateInfo);
    }
}