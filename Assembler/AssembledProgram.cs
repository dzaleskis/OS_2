using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OS_2.Concepts;
using OS_2.Utils;

namespace Assembler
{
    public class AssembledVariable
    {
        public int Value { get; set; }

        public AssembledVariable(int value)
        {
            Value = value;
        }
        public static string Serialize(AssembledVariable variable)
        {
            return variable.Value.ToShort().Serialize();
        }
    }
    
    public class AssembledInstruction
    {
        public Opcode Opcode { get; set; }
        public int Operand { get; set; }

        public AssembledInstruction(Opcode opcode, int operand = 0)
        {
            Opcode = opcode;
            Operand = operand;
        }

        public static string Serialize(AssembledInstruction instruction)
        {
            return ((int) instruction.Opcode).ToShort().Serialize() + instruction.Operand.ToShort().Serialize();
        }
    }
    
    public class AssembledProgram
    {
        public List<AssembledVariable> AssembledVariables { get; set; }
        public List<AssembledInstruction> AssembledInstructions { get; set; }
    }

    public class SerializedProgram
    {
        public string SerializedVariables { get; set; }
        public string SerializedInstructions { get; set; }

        public static SerializedProgram FromAssembledProgram(AssembledProgram assembledProgram)
        {
            return new SerializedProgram()
            {
                SerializedVariables = string.Join(null, assembledProgram.AssembledVariables.Select(AssembledVariable.Serialize)),
                SerializedInstructions = string.Join(null, assembledProgram.AssembledInstructions.Select(AssembledInstruction.Serialize))
            };
        }

        public byte[] GetVariablesAsBytes()
        {
            return Encoding.ASCII.GetBytes(SerializedVariables);
        }
        
        public byte[] GetInstructionsAsBytes()
        {
            return Encoding.ASCII.GetBytes(SerializedInstructions);
        }
        
    }
}