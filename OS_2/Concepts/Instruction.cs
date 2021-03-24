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
        INT,
        POPR,
        PUSHR,
        INB,
        OUTB
    }

    public class VoidInstruction
    {
        public Opcode Opcode { get; set; }

        public VoidInstruction(Opcode opcode)
        {
            Opcode = opcode;
        }
    }

    public class UnaryInstruction : VoidInstruction
    {
        public int A { get; set; }
        public UnaryInstruction(Opcode opcode, int a): base(opcode)
        {
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