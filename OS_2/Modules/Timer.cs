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
        public readonly InterruptLine InterruptLine;
        private bool _interruptsEnabled = false;
        
        public Timer(IInterruptController controller)
        {
            InterruptLine = new InterruptLine(controller);
        }

        public override void DoCycle()
        {
            if (_interruptsEnabled)
            {
                InterruptLine.TriggerInterrupt();
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
                    _interruptsEnabled = value > 0;
                    break;
                case (byte)TimerRegisters.Timeout:
                    UpdateTimer(value);
                    break;
            }
        }

        public byte ReadFrom(int accessedIndex)
        {
            switch (accessedIndex)
            {
                case (byte)TimerRegisters.EnableInterrupts:
                    return (byte)(_interruptsEnabled ? 1 : 0);
                case (byte)TimerRegisters.Timeout:
                    return (byte) Timeout;
            }

            return 0;
        }
    }
}