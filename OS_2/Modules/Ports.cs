using System;
using System.Collections.Generic;
using System.Linq;
using OS_2.Concepts;
using OS_2.Utils;

namespace OS_2.Modules
{
    internal class AllocatedDevice
    {
        public readonly int RequiredPorts;
        public readonly int StartPort;
        public readonly IPortDevice DeviceRef;

        public AllocatedDevice(int requiredPorts, int startPort, IPortDevice deviceRef)
        {
            this.RequiredPorts = requiredPorts;
            this.StartPort = startPort;
            this.DeviceRef = deviceRef;
        }
    }
    
    public class Ports
    {
        private List<AllocatedDevice> allocatedDevices = new List<AllocatedDevice>();
        private int lastAllocatedPort = 0;

        public void AllocateDevice(IPortDevice portDevice)
        {
            allocatedDevices.Add(new AllocatedDevice(portDevice.GetRequiredPorts(), lastAllocatedPort, portDevice));
            lastAllocatedPort += portDevice.GetRequiredPorts();
        }

        public void WriteToPort(ushort port, byte value)
        {
            var device = allocatedDevices
                .Single(device => IsPortInDeviceRange(port, device));

            device.DeviceRef.WriteTo(port - device.StartPort, value);
        }
        
        public byte ReadFromPort(ushort port)
        {
            var device = allocatedDevices
                .Single(device => IsPortInDeviceRange(port, device));

            return device.DeviceRef.ReadFrom(port - device.StartPort);
        }

        private bool IsPortInDeviceRange(ushort port, AllocatedDevice device)
        {
            var start = device.StartPort;
            var end = start + device.RequiredPorts;
            return port >= start && port <= end;
        }

        public void Reset()
        {
            allocatedDevices.Clear();
            lastAllocatedPort = 0;
        }
    }
}