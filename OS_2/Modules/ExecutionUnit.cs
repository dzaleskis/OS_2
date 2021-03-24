using System;
using OS_2.Concepts;
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
                    // if in protected mode, switch to real mode and continue execution?
                    // if in real mode, stop execution
                    // probably easiest to throw an exception and determine how to handle it in CPU
                    throw new Exception("HALT");
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
                    context.PCWrite(context.Operand);
                    break;
                case Opcode.JE:
                    if (flags.HasFlag(Flags.ZF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.JL:
                    if (flags.HasFlag(Flags.SF) != flags.HasFlag(Flags.OF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.JB:
                    if (flags.HasFlag(Flags.CF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.JLE:
                    if (flags.HasFlag(Flags.SF) != flags.HasFlag(Flags.OF) || flags.HasFlag(Flags.ZF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.JBE:
                    if (flags.HasFlag(Flags.CF) || flags.HasFlag(Flags.ZF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.JG:
                    if (!flags.HasFlag(Flags.ZF) && flags.HasFlag(Flags.SF) == flags.HasFlag(Flags.OF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.JA:
                    if (!flags.HasFlag(Flags.CF) && !flags.HasFlag(Flags.ZF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.JGE:
                    if (flags.HasFlag(Flags.SF) == flags.HasFlag(Flags.OF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.JAE:
                    if (!flags.HasFlag(Flags.CF))
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.LOOP:
                    var dec = stackTop - 1;
                    ReplaceTop(dec, context.MemWrite);
                    if (dec != 0)
                    {
                        context.PCWrite(context.Operand);
                    }
                    break;
                case Opcode.CALL:
                    Push(context.PCRead(), context.MemWrite);
                    context.PCWrite(context.Operand);
                    break;
                case Opcode.RET:
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
                case Opcode.OUTB:
                    context.PortWrite(context.Operand, stackTop);
                    break;
                default:
                    throw new ArgumentException("invalid opcode");
            }
        }

        private void Push(int value, Action<int, int> memWriteFunc)
        {
            SP += Constants.WORD_LENGTH;
            ReplaceTop(value, memWriteFunc);
        }

        private void Pop()
        {
            SP -= Constants.WORD_LENGTH;
        }
        
        private void ReplaceTop(int value, Action<int, int> memWriteFunc)
        {
            memWriteFunc(SP, value);
        }
    }
}