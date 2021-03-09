using OS_2.Modules;

namespace OS_2.Concepts
{
    public class InterruptLine
    {
        private IInterruptController _controller;
        public InterruptLine(IInterruptController controller)
        {
            _controller = controller;
            _controller.RegisterInterruptLine(this);
        }

        public void TriggerInterrupt()
        {
            _controller.HandleInterruptRequest(this);
        }
    }
}