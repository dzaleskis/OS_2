using System.Threading;
using OS_2.IO;
using OS_2.Modules;
using OS_2.Utils;
using Monitor = OS_2.IO.Monitor;
using Timer = OS_2.Modules.Timer;

namespace OS_2.Machines
{
    public class Machine
    {
        private Memory Memory = new Memory();
        private Ports Ports = new Ports();
        private CPU CPU;
        private Timer Timer;
        private FloppyDrive FloppyDrive;
        private Monitor Monitor;

        public Machine()
        {
            CPU = new CPU(Memory, Ports);
            Timer = new Timer(CPU.InterruptController);
            Ports.AllocateDevice(Timer);
            FloppyDrive = new FloppyDrive(Constants.FLOPPY_FILENAME, CPU.InterruptController);
            Ports.AllocateDevice(FloppyDrive);
            Monitor = new Monitor();
            Ports.AllocateDevice(Monitor);
        }

        public void StartDevices()
        {
            FloppyDrive.StartRunning();
            Monitor.StartRunning();
        }
        
        public void StopDevices()
        {
            FloppyDrive.StopRunning();
            Monitor.StopRunning();
        }

        public void ExecuteCycle()
        {
            
        }
    }
}