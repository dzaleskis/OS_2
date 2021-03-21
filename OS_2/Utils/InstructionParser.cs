using System;
using OS_2.Concepts;

namespace OS_2.Utils
{
    public static class InstructionParser
    {
        public static Opcode ParseOpcode(byte[] rawOpcode)
        {
            if (rawOpcode.Length != Constants.INSTRUCTION_OPCODE_LENGTH)
            {
                throw new ArgumentException("Invalid opcode length");
            }
            var converted = (Opcode) BitConverter.ToInt16(rawOpcode);
            
            if (!Enum.IsDefined(typeof(Opcode), converted))
            {
                throw new ArgumentException("Opcode is not defined");
            }
                
            return converted;
        }
        
        public static int ParseOperand(byte[] rawOperand)
        {
            if (rawOperand.Length != Constants.INSTRUCTION_OPERAND_LENGTH)
            {
                throw new ArgumentException("Invalid operand length");
            }
            
            int converted = BitConverter.ToInt16(rawOperand);

            return converted;
        }
    }
}