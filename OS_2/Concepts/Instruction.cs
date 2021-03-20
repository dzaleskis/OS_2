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
        JLE,
        JG,
        JGE,
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
    }

    public class UnaryInstruction : VoidInstruction
    {
        public int A { get; set; }
    }

    public class BinaryInstruction : UnaryInstruction
    {
        public int B { get; set; }
    }
}