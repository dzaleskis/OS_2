using System;
using System.Threading;

namespace OS_2.Concepts
{
    public abstract class AbstractCycleDevice: IDisposable
    {
        protected int Timeout { get; set; } = 100;
        
        protected Timer Timer;

        protected void UpdateTimer()
        {
            Timer.Change(0, Timeout);
        }

        public void StartRunning()
        {
            Dispose();
            Timer = new Timer((object stateInfo) => DoCycle(), null, 0, Timeout);
        }

        public void StopRunning()
        {
            Dispose();
        }
        
        protected abstract void DoCycle();

        public void Dispose()
        {
            Timer?.Dispose();
        }
    }
}