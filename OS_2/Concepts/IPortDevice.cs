namespace OS_2.Concepts
{
    public interface IPortDevice
    {
        int GetRequiredPorts();
        void WriteTo(int accessedIndex, byte value);
        byte ReadFrom(int accessedIndex);
    }
}