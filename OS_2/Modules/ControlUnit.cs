using System;
using System.Linq;
using OS_2.Concepts;
using OS_2.Utils;

namespace OS_2.Modules
{
    public class ControlUnit
    {
        private int PC { get; set; }
        public Opcode Opcode { get; private set; }
        public int Operand { get; private set; }
        
        private byte[] _instructionBytes;
        
        public void ParseInstruction()
        {
            if (_instructionBytes.Length != Constants.INSTRUCTION__LENGTH)
            {
                throw new ArgumentException($"Instruction length is {_instructionBytes.Length}, should be {Constants.INSTRUCTION__LENGTH}");
            }

            var opcodeBytes = _instructionBytes.Take(2).ToArray();
            var operandBytes = _instructionBytes.Skip(2).Take(2).ToArray();
                
            Opcode = InstructionParser.ParseOpcode(opcodeBytes);
            Operand = InstructionParser.ParseOperand(operandBytes);
        }

        public void ReadInstruction(Func<int, byte[]> memAccessFunc)
        {
            _instructionBytes = memAccessFunc(PC);
            PC += Constants.INSTRUCTION__LENGTH;
        }
    }
}