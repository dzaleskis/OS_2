using System;
using OS_2.Concepts;
using OS_2.Modules;
using OS_2.Utils;

namespace OS_2.Machines
{
    public enum ExecutionMode
    {
        Real,
        Protected // anything else than 0
    }
    
    public class CPU: ICycleDevice
    {
        private readonly ControlUnit _controlUnit = new ControlUnit();
        private readonly ExecutionUnit _executionUnit = new ExecutionUnit();
        private readonly Ports _ports;
        private readonly Memory _memory;
        public readonly InterruptController InterruptController = new InterruptController();
        private readonly MemoryManagementUnit _mmu;

        public CPU(Memory memory, Ports ports)
        {
            _memory = memory;
            _ports = ports;
            _mmu = new MemoryManagementUnit(_memory);
        }
        
        private Func<int, int> GetMemReadFunc()
        {
            if (_executionUnit.CR0 == (int)ExecutionMode.Real)
            {
                return (realAddress) =>_memory[realAddress];
            }
            
            // protected mode
            return (virtualAddress) =>
            {
                var realAddress = _mmu.ConvertVirtualToReal(virtualAddress);
                return _memory[realAddress];
            };
        }
        
        private Action<int, int> GetMemWriteFunc()
        {
            if (_executionUnit.CR0 == (int)ExecutionMode.Real)
            {
                return (realAddress, value) =>_memory[realAddress] = value;
            }
            
            // protected mode
            return (virtualAddress, value) =>
            {
                var realAddress = _mmu.ConvertVirtualToReal(virtualAddress);
                _memory[realAddress] = value;
            };
        }
        
        private Func<int, byte[]> GetMemBytesAccessFunc(int byteCount)
        {
            if (_executionUnit.CR0 == (int)ExecutionMode.Real)
            {
                return (realAddress) => _memory.Read(realAddress, byteCount);
            }
            
            // protected mode
            return (virtualAddress) =>
            {
                var realAddress = _mmu.ConvertVirtualToReal(virtualAddress);
                return _memory.Read(realAddress, byteCount);
            };
        }

        private void ParseInstruction()
        {
            _controlUnit.ParseInstruction();
        }

        private void ReadInstruction()
        {
            _controlUnit.ReadInstruction(GetMemBytesAccessFunc(Constants.INSTRUCTION__LENGTH));
        }

        private void ExecuteInstruction()
        {
            _executionUnit.ExecuteInstruction(_controlUnit.Opcode, _controlUnit.Operand, GetMemReadFunc(), GetMemWriteFunc());
        }

        public void DoCycle()
        {
            ReadInstruction();
            ParseInstruction();
            ExecuteInstruction();
        }
    }
}