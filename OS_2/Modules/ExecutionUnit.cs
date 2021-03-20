using OS_2.Concepts;

namespace OS_2.Modules
{
    public class ExecutionUnit
    {
        public int MEM { get; set; }
        public byte CR0 { get; set; }
        public int CR1 { get; set; }
        public int BP { get; set; }
        public int SP { get; set; }
        public byte INTR { get; set; }
        public int IDT { get; set; }
        public ALU ALU = new ALU();

        public void ExecuteInstruction(Opcode opcode)
        {
            switch (opcode)
            {

            }
        }
    }
}