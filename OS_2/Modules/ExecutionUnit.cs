using System;
using OS_2.Concepts;
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
        private readonly ALU ALU = new ALU();

        public void ExecuteInstruction(Opcode opcode, int operand, Func<int, int> memReadFunc, Action<int, int> memWriteFunc)
        {
            var stackTop = memReadFunc(SP);
            var stackTop2 = memReadFunc(SP - Constants.WORD_LENGTH);
            int res = 0;
            
            switch (opcode)
            {
                case Opcode.HALT:
                    // if in protected mode, switch to real mode and continue execution?
                    // if in real mode, stop execution
                case Opcode.ADD:
                case Opcode.SUB:
                case Opcode.MUL:
                case Opcode.IMUL:
                case Opcode.DIV:
                case Opcode.IDIV:
                case Opcode.AND:
                case Opcode.OR:
                case Opcode.XOR:
                    var binaryInstr = new BinaryInstruction(opcode, stackTop2, stackTop);
                    res = ALU.Process(binaryInstr);
                    SP -= Constants.WORD_LENGTH;
                    Replace(res, memWriteFunc);
                    break;
                case Opcode.NOT:
                    var unaryInstr = new UnaryInstruction(opcode, stackTop);
                    res = ALU.Process(unaryInstr);
                    Replace(res, memWriteFunc);
                    break;
                case Opcode.CMP:
                    // only need to set flags
                    var subInstr = new BinaryInstruction(Opcode.SUB, stackTop2, stackTop);
                    ALU.Process(subInstr);
                    break;
                case Opcode.LOD:
                    var memoryVal = memReadFunc(operand);
                    Push(memoryVal, memWriteFunc);
                    break;
                case Opcode.LODE:
                    var spAddress = memReadFunc(stackTop);
                    var spMemoryVal = memReadFunc(spAddress);
                    Push(spMemoryVal, memWriteFunc);
                    break;
                case Opcode.STO:
                    memWriteFunc(operand, stackTop);
                    break;
                case Opcode.STOE:
                    var sp2Address = memReadFunc(stackTop2);
                    memWriteFunc(sp2Address, stackTop);
                    break;
                case Opcode.PUSH:
                    Push(operand, memWriteFunc);
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
                    INTR = unchecked((byte)operand);
                    break;
                case Opcode.POPR:
                    // need some kind of indexing system for registers
                case Opcode.PUSHR:
                case Opcode.INB:
                    // need access to ports
                case Opcode.OUTB:
                    
                default:
                    throw new ArgumentException("invalid opcode");
            }
        }

        private void Push(int value, Action<int, int> memWriteFunc)
        {
            SP += Constants.WORD_LENGTH;
            memWriteFunc(SP, value);
        }

        private void Pop()
        {
            SP -= Constants.WORD_LENGTH;
        }
        
        private void Replace(int value, Action<int, int> memWriteFunc)
        {
            memWriteFunc(SP, value);
        }
    }
}