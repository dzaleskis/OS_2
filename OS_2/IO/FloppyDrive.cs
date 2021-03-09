using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OS_2.Concepts;
using OS_2.Machines;
using OS_2.Modules;

namespace OS_2.IO
{
    public enum FloppyRegister
    {
        Control,
        Error,
        LBA, // LBA Addressing lets us address blocks by integers and 256 blocks * 32 bytes = 8192 of addressable storage
        DataFIFO, // this register is actually a stack that contains 32 bytes
    }

    public enum FloppyError
    {
        IncorrectAccess = 1,
        NothingToRead = 2,
    }
    
    public enum FloppyControl
    {
        NoAction,
        ReadData,
    }
    
    public class FloppyDrive: AbstractCycleDevice, IPortDevice
    {
        private const int REGISTER_COUNT = 4;
        private const int BLOCK_SIZE = 32;

        private byte _control;
        private byte _error;
        private byte _lba;
        private readonly Queue<byte> _dataFifo = new Queue<byte>();
        
        private readonly string _driveFilename;
        private readonly InterruptLine _interruptLine;

        public FloppyDrive(string filename, IInterruptController controller)
        {
            _driveFilename = filename;
            _interruptLine = new InterruptLine(controller);
        }
        
        public int GetRequiredPorts()
        {
            return REGISTER_COUNT;
        }

        public void WriteTo(int accessedIndex, byte value)
        {
            switch (accessedIndex)
            {
                case (byte)FloppyRegister.Control:
                    _control = value;
                    break;
                case (byte)FloppyRegister.DataFIFO:
                    SetError(FloppyError.IncorrectAccess);
                    break;
                case (byte)FloppyRegister.Error:
                    _error = value;
                    break;
                case (byte)FloppyRegister.LBA:
                    _lba = value;
                    break;
            }
        }

        public byte ReadFrom(int accessedIndex)
        {
            switch (accessedIndex)
            {
                case (byte)FloppyRegister.Control:
                    return _control;
                
                case (byte)FloppyRegister.DataFIFO:
                    if (_dataFifo.Count == 0)
                    {
                        SetError(FloppyError.NothingToRead);
                        break;
                    }
                    return _dataFifo.Dequeue();
                
                case (byte)FloppyRegister.Error:
                    return _error;
                
                case (byte)FloppyRegister.LBA:
                    return _lba;
            }
            return 0;
        }

        private void SetError(FloppyError errorCode)
        {
            _error = (byte) errorCode;
        }

        private void ReadFromDisk(int index)
        {
            var file = File.OpenRead(_driveFilename);
            file.Seek(index * BLOCK_SIZE, SeekOrigin.Begin);

            var readBuffer = new byte[BLOCK_SIZE];
            var bytesRead = file.Read(readBuffer);

            foreach (var i in Enumerable.Range(0, bytesRead))
            {
                _dataFifo.Enqueue(readBuffer[i]);
            }
            
            file.Close();
        }

        protected override void DoCycle()
        {
            // do on every cycle:
            // if control register is set to ReadData and queue is empty
            // read the data to fifo, reset control register, then trigger an interrupt
            if (_control == (byte) FloppyControl.ReadData)
            {
                ReadFromDisk(_lba);
                _control = (byte) FloppyControl.NoAction;
                _interruptLine.TriggerInterrupt();
            }
        }
    }
}