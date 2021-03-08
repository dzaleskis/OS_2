using OS_2.Concepts;
using OS_2.Machines;

namespace OS_2.IO
{
    public class FloppyDrive: IPortDevice
    {
        public int GetRequiredPorts()
        {
            throw new System.NotImplementedException();
        }

        public void WriteTo(int accessedIndex, byte value)
        {
            throw new System.NotImplementedException();
        }

        public byte ReadFrom(int accessedIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}