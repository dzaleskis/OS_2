using System;
using OS_2.Concepts;
using OS_2.Exceptions;
using OS_2.Machines;
using OS_2.Utils;

namespace OS_2.Modules
{
    public class ExecutionUnit
    {
        public byte CR0 { get; set; }
        public int BP { get; set; }
        public int SP { get; set; }
        public byte INTR { get; set; }
        public int IDT { get; set; }
        
        public readonly ALU ALU = new ALU();

        public void ExecuteInstruction(InstructionExecutionContext context)
        {
            var stackTop = context.MemRead(SP);
            var stackTop2 = context.MemRead(SP - Constants.WORD_LENGTH);
            var flags = ALU.Flags;
            
            switch (context.Opcode)
            {
                case Opcode.HALT:
                    throw new HaltException();
                case Opcode.ADD:
                case Opcode.SUB:
                case Opcode.MUL:
                case Opcode.IMUL:
                case Opcode.DIV:
                case Opcode.IDIV:
                case Opcode.AND:
                case Opcode.OR:
                case Opcode.XOR:
                    var binaryInstr = new BinaryInstruction(context.Opcode, stackTop2, stackTop);
                    var binRes = ALU.Process(binaryInstr);
                    Pop();
                    ReplaceTop(binRes, context.MemWrite);
                    break;
                case Opcode.NOT:
                    var unaryInstr = new UnaryInstruction(context.Opcode, stackTop);
                    var unRes = ALU.Process(unaryInstr);
                    ReplaceTop(unRes, context.MemWrite);
                    break;
                case Opcode.CMP:
                    // only need to set flags
                    var subInstr = new BinaryInstruction(Opcode.SUB, stackTop2, stackTop);
                    ALU.Process(subInstr);
                    break;
                case Opcode.LOD:
                    var memoryVal = context.MemRead(context.Operand);
                    Push(memoryVal, context.MemWrite);
                    break;
                case Opcode.LODE:
                    var spAddress = context.MemRead(stackTop);
                    var spMemoryVal = context.MemRead(spAddress);
                    Push(spMemoryVal, context.MemWrite);
                    break;
                case Opcode.STO:
                    context.MemWrite(context.Operand, stackTop);
                    break;
                case Opcode.STOE:
                    var sp2Address = context.MemRead(stackTop2);
                    context.MemWrite(sp2Address, stackTop);
                    break;
                case Opcode.PUSH:
                    Push(context.Operand, context.MemWrite);
                    break;
                case Opcode.POP:
                    Pop();
                    break;
                case Opcode.JMP:
                    Jump(context.Operand, context.PCWrite);
                    break;
                case Opcode.JE:
                    if (flags.HasFlag(Flags.ZF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.JL:
                    if (flags.HasFlag(Flags.SF) != flags.HasFlag(Flags.OF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.JB:
                    if (flags.HasFlag(Flags.CF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.JLE:
                    if (flags.HasFlag(Flags.SF) != flags.HasFlag(Flags.OF) || flags.HasFlag(Flags.ZF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.JBE:
                    if (flags.HasFlag(Flags.CF) || flags.HasFlag(Flags.ZF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.JG:
                    if (!flags.HasFlag(Flags.ZF) && flags.HasFlag(Flags.SF) == flags.HasFlag(Flags.OF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.JA:
                    if (!flags.HasFlag(Flags.CF) && !flags.HasFlag(Flags.ZF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.JGE:
                    if (flags.HasFlag(Flags.SF) == flags.HasFlag(Flags.OF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.JAE:
                    if (!flags.HasFlag(Flags.CF))
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.LOOP:
                    var dec = stackTop - 1;
                    ReplaceTop(dec, context.MemWrite);
                    if (dec != 0)
                    {
                        Jump(context.Operand, context.PCWrite);
                    }
                    break;
                case Opcode.CALL:
                    Push(context.PCRead(), context.MemWrite);
                    Jump(context.Operand, context.PCWrite);
                    break;
                case Opcode.RET:
                    context.PCWrite(stackTop);
                    Pop();
                    break;
                case Opcode.IRET:
                    context.RegWrite((int)RealRegister.FLAGS, stackTop);
                    Pop();
                    context.PCWrite(stackTop);
                    Pop();
                    break;
                case Opcode.INT:
                    INTR = unchecked((byte)context.Operand);
                    break;
                case Opcode.POPR:
                    context.RegWrite(context.Operand, stackTop);
                    Pop();
                    break;
                case Opcode.PUSHR:
                    var registerValue = context.RegRead(context.Operand);
                    Push(registerValue, context.MemWrite);
                    break;
                case Opcode.INB:
                    var portValue = context.PortRead(context.Operand);
                    Push(portValue, context.MemWrite);
                    break;
                case Opcode.INBE:
                    var stackPortValue = context.PortRead(stackTop);
                    Push(stackPortValue, context.MemWrite);
                    break;
                case Opcode.OUTB:
                    context.PortWrite(context.Operand, stackTop);
                    break;
                case Opcode.OUTBE:
                    context.PortWrite(stackTop2, stackTop);
                    break;
                default:
                    throw new ArgumentException("invalid opcode");
            }
        }

        public void Jump(int address, Action<int> pcWriteFunc)
        {
            pcWriteFunc(address);
        }

        public void Push(int value, Action<int, int> memWriteFunc)
        {
            SP += Constants.WORD_LENGTH;
            ReplaceTop(value, memWriteFunc);
        }

        public void Pop()
        {
            SP -= Constants.WORD_LENGTH;
        }
        
        public void ReplaceTop(int value, Action<int, int> memWriteFunc)
        {
            memWriteFunc(SP, value);
        }
    }
}