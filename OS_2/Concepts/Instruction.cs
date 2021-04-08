using System;

namespace OS_2.Concepts
{
    public enum Opcode
    {
        HALT,
        ADD,
        SUB,
        MUL,
        IMUL,
        DIV,
        IDIV,
        AND,
        OR,
        XOR,
        NOT,
        CMP,
        LOD,
        LODE,
        STO,
        STOE,
        PUSH,
        POP,
        JMP,
        JE,
        JL,
        JB,
        JLE,
        JBE,
        JG,
        JA,
        JGE,
        JAE,
        LOOP,
        CALL,
        RET,
        IRET,
        INT,
        POPR,
        PUSHR,
        INB,
        OUTB
    }

    public class Instruction
    {
        public Opcode Opcode { get; set; }
        public int Operand { get; set; }
    }

    public class UnaryInstruction
    {
        public Opcode Opcode { get; set; }
        public int A { get; set; }
        public UnaryInstruction(Opcode opcode, int a)
        {
            Opcode = opcode;
            A = a;
        }
    }

    public class BinaryInstruction : UnaryInstruction
    {
        public int B { get; set; }

        public BinaryInstruction(Opcode opcode, int a, int b): base(opcode, a)
        {
            B = b;
        }
    }
}