using System;
using System.Linq;
using OS_2.Concepts;
using OS_2.Utils;

namespace OS_2.Modules
{
    public class ControlUnit
    {
        public int PC { get; private set; }
        public Opcode Opcode { get; private set; }
        public int Operand { get; private set; }
        
        private void ParseInstruction(byte[] instruction)
        {
            if (instruction.Length != Constants.INSTRUCTION__LENGTH)
            {
                throw new ArgumentException($"Instruction length is {instruction.Length}, should be {Constants.INSTRUCTION__LENGTH}");
            }

            var opcodeBytes = instruction.Take(2).ToArray();
            var operandBytes = instruction.Skip(2).Take(2).ToArray();
                
            Opcode = InstructionParser.ParseOpcode(opcodeBytes);
            Operand = InstructionParser.ParseOperand(operandBytes);
            PC += instruction.Length;
        }
    }
}