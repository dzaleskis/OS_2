using System.Collections.Generic;
using OS_2.Concepts;

namespace Assembler
{
    public class NotAssembledVariable
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public NotAssembledVariable(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    public class NotAssembledInstruction
    {
        public string Name { get; set; }
        public string Operand { get; set; }
        public NotAssembledInstruction(string name, string operand = null)
        {
            Name = name;
            Operand = operand;
        }
    }
    
    public class NotAssembledProgram
    {
        public List<NotAssembledVariable> Variables { get; set; }
        public List<NotAssembledInstruction> Instructions { get; set; }
    }
}