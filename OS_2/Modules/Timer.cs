using OS_2.Concepts;

namespace OS_2.Modules
{
    public enum TimerRegisters
    {
        EnableInterrupts, // 0 means disabled
        Timeout // 1 to 255 ms
    }
    
    public class Timer: AbstractCycleDevice, IPortDevice
    {
        private const int REGISTER_COUNT = 2;
        public readonly InterruptLine _interruptLine;
        private bool InterruptsEnabled = false;
        
        public Timer(IInterruptController controller)
        {
            _interruptLine = new InterruptLine(controller);
        }

        protected override void DoCycle()
        {
            if (InterruptsEnabled)
            {
                _interruptLine.TriggerInterrupt();
            }
        }

        public int GetRequiredPorts()
        {
            return REGISTER_COUNT;
        }

        public void WriteTo(int accessedIndex, byte value)
        {
            switch (accessedIndex)
            {
                case (byte)TimerRegisters.EnableInterrupts:
                    InterruptsEnabled = value > 0;
                    break;
                case (byte)TimerRegisters.Timeout:
                    Timeout = value;
                    UpdateTimer();
                    break;
            }
        }

        public byte ReadFrom(int accessedIndex)
        {
            switch (accessedIndex)
            {
                case (byte)TimerRegisters.EnableInterrupts:
                    return (byte)(InterruptsEnabled ? 1 : 0);
                case (byte)TimerRegisters.Timeout:
                    return (byte) Timeout;
            }

            return 0;
        }
    }
}