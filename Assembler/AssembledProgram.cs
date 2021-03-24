using System.Collections.Generic;
using OS_2.Concepts;

namespace Assembler
{
    public class AssembledVariable
    {
        public int Value { get; set; }
    }
    
    public class AssembledInstruction
    {
        public Opcode Opcode { get; set; }
        public int Operand { get; set; }
    }
    
    public class AssembledProgram
    {
        public List<AssembledVariable> AssembledVariables { get; set; }
        public List<AssembledInstruction> AssembledInstructions { get; set; }
    }
}