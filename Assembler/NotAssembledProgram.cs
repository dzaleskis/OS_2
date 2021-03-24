using System.Collections.Generic;
using OS_2.Concepts;

namespace Assembler
{
    public class StaticVariable
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class NotAssembledInstruction
    {
        public string Name { get; set; }
        public string Operand { get; set; }
    }
    
    public class NotAssembledProgram
    {
        public List<StaticVariable> Variables { get; set; }
        public List<NotAssembledInstruction> Instructions { get; set; }
    }
}