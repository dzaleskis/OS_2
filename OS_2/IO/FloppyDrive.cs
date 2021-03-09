using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OS_2.Concepts;
using OS_2.Machines;

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
        private const int FIFO_INDEX = REGISTER_COUNT - 1;
        private const int BLOCK_SIZE = 32;
        
        private readonly byte[] _byteRegisters = new byte[REGISTER_COUNT - 1];
        private readonly Queue<byte> _dataFifo = new Queue<byte>();
        private readonly string _deviceFilename;

        public FloppyDrive(string filename)
        {
            _deviceFilename = filename;
        }
        
        public int GetRequiredPorts()
        {
            return REGISTER_COUNT;
        }

        public void WriteTo(int accessedIndex, byte value)
        {
            if (accessedIndex != FIFO_INDEX)
            {
                _byteRegisters[accessedIndex] = value;
            }
            else
            {
                SetError(FloppyError.IncorrectAccess);
            }
        }

        public byte ReadFrom(int accessedIndex)
        {
            if (accessedIndex != FIFO_INDEX)
            {
                return _byteRegisters[accessedIndex];
            }

            if (_dataFifo.Count != 0) return _dataFifo.Dequeue();
            
            SetError(FloppyError.NothingToRead);
            return 0;
        }

        private void SetError(FloppyError errorCode)
        {
            _byteRegisters[(int) FloppyRegister.Error] = (byte) errorCode;
        }

        private void ReadFromDisk(int index)
        {
            var file = File.OpenRead(_deviceFilename);
            file.Seek(index * BLOCK_SIZE, SeekOrigin.Begin);

            var readBuffer = new byte[BLOCK_SIZE];
            var bytesRead = file.Read(readBuffer);

            foreach (var i in Enumerable.Range(0, bytesRead))
            {
                _dataFifo.Enqueue(readBuffer[i]);
            }
            
            file.Close();
        }

        protected override void DoCycle(object stateInfo)
        {
            // do on every cycle:
            // if control register is set to ReadData and queue is empty
            // read the data to fifo, reset control register, then trigger an interrupt
            if (_byteRegisters[(int) FloppyRegister.Control] == (byte) FloppyControl.ReadData)
            {
                ReadFromDisk(_byteRegisters[(int) FloppyRegister.LBA]);
                _byteRegisters[(int) FloppyRegister.Control] = (byte) FloppyControl.NoAction;
                // TODO: implement interrupt mechanism
            }
        }
    }
}