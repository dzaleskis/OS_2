namespace OS_2.Concepts
{
    public interface IInterruptController
    {
        public void RegisterInterruptLine(InterruptLine line);

        public void HandleInterruptRequest(InterruptLine line);

        public byte ReadInterruptRequests();
    }
}