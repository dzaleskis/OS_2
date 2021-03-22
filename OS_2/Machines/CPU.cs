using System;
using OS_2.Concepts;
using OS_2.Modules;
using OS_2.Utils;

namespace OS_2.Machines
{
    public class InstructionExecutionContext
    {
        public Opcode Opcode;
        public int Operand;
        public Func<int, int> MemRead;
        public Action<int, int> MemWrite;
        public Func<int, int> PortRead;
        public Action<int, int> PortWrite;
        public Func<int, int> RegRead;
        public Action<int, int> RegWrite;
        public Action<int> Jump;
    }
    
    public class CPU: ICycleDevice
    {
        private readonly ControlUnit _controlUnit = new ControlUnit();
        private readonly ExecutionUnit _executionUnit = new ExecutionUnit();
        private readonly Ports _ports;
        private readonly Memory _memory;
        public readonly InterruptController InterruptController = new InterruptController();
        private readonly MemoryManagementUnit _mmu;
        
        private const int REAL_MODE = 0;

        public CPU(Memory memory, Ports ports)
        {
            _memory = memory;
            _ports = ports;
            _mmu = new MemoryManagementUnit(_memory);
        }
        
        private Func<int, int> GetMemReadFunc()
        {
            if (_executionUnit.CR0 == REAL_MODE)
            {
                return (realAddress) =>_memory[realAddress.ToUshort()];
            }
            
            // protected mode
            return (virtualAddress) =>
            {
                var realAddress = _mmu.ConvertVirtualToReal(virtualAddress.ToUshort());
                return _memory[realAddress.ToUshort()];
            };
        }
        
        private Action<int, int> GetMemWriteFunc()
        {
            if (_executionUnit.CR0 == REAL_MODE)
            {
                return (realAddress, value) =>_memory[realAddress.ToUshort()] = value;
            }
            
            // protected mode
            return (virtualAddress, value) =>
            {
                var realAddress = _mmu.ConvertVirtualToReal(virtualAddress.ToUshort());
                _memory[realAddress.ToUshort()] = value;
            };
        }
        
        private Func<int, int> GetRegReadFunc()
        {
            if (_executionUnit.CR0 == REAL_MODE)
            {
                return (realRegister) =>
                {
                    switch ((RealRegister) realRegister)
                    {
                        case RealRegister.BP:
                            return _executionUnit.BP;
                        case RealRegister.CR0:
                            return _executionUnit.CR0;
                        case RealRegister.CR1:
                            return _mmu.CR1;
                        case RealRegister.SP:
                            return _executionUnit.SP;
                        case RealRegister.IDT:
                            return _executionUnit.IDT;
                        case RealRegister.FLAGS:
                            return (int)_executionUnit.ALU.Flags;
                        case RealRegister.PT:
                            return _mmu.PT;
                        default:
                            throw new Exception("invalid read of real register");
                    }
                };
            }
            
            // protected mode
            return (protectedRegister) =>
            {
                switch ((ProtectedRegister) protectedRegister)
                {
                    case ProtectedRegister.BP:
                        return _executionUnit.BP;
                    case ProtectedRegister.SP:
                        return _executionUnit.SP;
                    case ProtectedRegister.FLAGS:
                        return (int)_executionUnit.ALU.Flags;

                    default:
                        throw new Exception("invalid read of protected register");
                }
            };
        }
        
        private Action<int, int> GetRegWriteFunc()
        {
            if (_executionUnit.CR0 == REAL_MODE)
            {
                return (realRegister, value) =>
                {
                    switch ((RealRegister) realRegister)
                    {
                        case RealRegister.BP:
                            _executionUnit.BP = value;
                            break;
                        case RealRegister.CR0:
                            _executionUnit.CR0 = unchecked((byte)value);
                            break;
                        case RealRegister.CR1:
                            _mmu.CR1 = value;
                            break;
                        case RealRegister.SP:
                            _executionUnit.SP = value;
                            break;
                        case RealRegister.IDT:
                            _executionUnit.IDT = value;
                            break;
                        case RealRegister.FLAGS:
                            _executionUnit.ALU.Flags = (Flags)value;
                            break;
                        case RealRegister.PT:
                            _mmu.PT = value;
                            break;
                        default:
                            throw new Exception("invalid write to real register");
                    }
                };
            }
            
            // protected mode
            return (protectedRegister, value) =>
            {
                switch ((ProtectedRegister) protectedRegister)
                {
                    case ProtectedRegister.BP:
                        _executionUnit.BP = value;
                        break;
                    case ProtectedRegister.SP:
                        _executionUnit.SP = value;
                        break;
                    case ProtectedRegister.FLAGS:
                        _executionUnit.ALU.Flags = (Flags)value;
                        break;
                    default:
                        throw new Exception("invalid write to protected register");
                }
            };
        }
        
        private Func<int, byte[]> GetMemBytesAccessFunc(int byteCount)
        {
            if (_executionUnit.CR0 == REAL_MODE)
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
            // TODO: implement exception handling
            _executionUnit.ExecuteInstruction(ConstructContext());
            // TODO: implement interrupt handling
        }

        private InstructionExecutionContext ConstructContext()
        {
            return new InstructionExecutionContext()
            {
                Opcode = _controlUnit.Opcode,
                Operand = _controlUnit.Operand,
                MemRead = GetMemReadFunc(),
                MemWrite = GetMemWriteFunc(),
                PortRead = (port) => _ports.ReadFromPort(port),
                PortWrite = (port, value) => _ports.WriteToPort(port, unchecked((byte)value)),
                RegRead = GetRegReadFunc(),
                RegWrite = GetRegWriteFunc(),
                Jump = (address) => _controlUnit.Jump(address)
            };
        }

        public void DoCycle()
        {
            ReadInstruction();
            ParseInstruction();
            ExecuteInstruction();
        }
    }
}