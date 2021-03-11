using System;
using System.Threading;
using OS_2.Utils;

namespace OS_2.Concepts
{
    public abstract class AbstractCycleDevice: IDisposable
    {
        protected int Timeout { get; private set; } = Constants.DEFAULT_TIMEOUT;
        private Timer _timer;

        protected void UpdateTimer(int timeout)
        {
            Timeout = timeout;
            _timer.Change(0, timeout);
        }

        public void StartRunning()
        {
            Dispose();
            _timer = new Timer((object stateInfo) => DoCycle(), null, 0, Timeout);
        }

        public void StopRunning()
        {
            Dispose();
        }
        
        protected abstract void DoCycle();

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}