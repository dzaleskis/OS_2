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
                case Opcode.JE:
                case Opcode.JL:
                case Opcode.JLE:
                case Opcode.JG:
                case Opcode.JGE:
                case Opcode.LOOP:
                case Opcode.CALL:
                case Opcode.RET:
                    // these all suck
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