using System;
using OS_2.Concepts;
using OS_2.Exceptions;
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
        public Action<int> PCWrite;
        public Func<int> PCRead;
    }

    public enum InterruptCode
    {
        HaltException,
        PageException
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
        
        public void DoCycle()
        {
            try
            {
                ReadInstruction();
                ParseInstruction();
                ExecuteInstruction();
            }
            catch (HaltException)
            {
                if (IsRealMode()) throw;
                _executionUnit.INTR = (byte) InterruptCode.HaltException;
            }
            catch (PageException)
            {
                if (IsRealMode()) throw;
                _executionUnit.INTR = (byte) InterruptCode.PageException;
            }

            if (_executionUnit.INTR > 0)
            {
                HandleInterrupt(_executionUnit.INTR);
                _executionUnit.INTR = 0;
            }

            if (InterruptController.IRQ > 0)
            {
                HandleInterrupt(InterruptController.IRQ);
                InterruptController.IRQ = 0;   
            }
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
            _executionUnit.ExecuteInstruction(ConstructContext());
        }

        private void HandleInterrupt(byte interruptCode)
        {
            var offset = interruptCode * Constants.WORD_LENGTH;
            _executionUnit.Push((int)_executionUnit.ALU.Flags, GetMemWriteFunc());
            _executionUnit.Jump(_executionUnit.IDT + offset, (address) => _controlUnit.PC = address);
        }

        private bool IsRealMode() => _executionUnit.CR0 == 0;
        
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
                PCWrite = (address) => _controlUnit.PC = address,
                PCRead = () => _controlUnit.PC
            };
        }

        private Func<int, int> GetMemReadFunc()
        {
            if (IsRealMode())
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
            if (IsRealMode())
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
            if (IsRealMode())
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
                            throw new RegisterAccessException("invalid read of real register");
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
                        throw new RegisterAccessException("invalid read of protected register");
                }
            };
        }
        
        private Action<int, int> GetRegWriteFunc()
        {
            if (IsRealMode())
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
                            throw new RegisterAccessException("invalid write to real register");
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
                        throw new RegisterAccessException("invalid write to protected register");
                }
            };
        }
        
        private Func<int, byte[]> GetMemBytesAccessFunc(int byteCount)
        {
            if (IsRealMode())
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
        
        
    }
}