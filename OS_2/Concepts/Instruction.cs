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

    public enum ArithmeticOpcode
    {
        ADD = Opcode.ADD,
        SUB = Opcode.SUB,
        MUL = Opcode.MUL,
        IMUL = Opcode.IMUL,
        DIV = Opcode.DIV,
        IDIV = Opcode.IDIV
    }
    
    public enum LogicalOpcode
    {
        AND = Opcode.AND,
        OR = Opcode.OR,
        XOR = Opcode.XOR,
        NOT = Opcode.NOT,
    }
    
    public class Instruction
    {
        public Opcode Opcode { get; set; }
    }

    public class Operand
    {
        public int Value { get; set; } 
    }
    
    public class InstructionResult
    {
        public InstructionResult(int value)
        {
            Value = value;
        }

        private int Value { get; set; }
    }

    
    public class VoidInstruction : Instruction {}

    public class UnaryInstruction : Instruction
    {
        public Operand Operand1 { get; set; }
    }

    public class BinaryInstruction : Instruction
    {
        public Operand Operand1 { get; set; }
        public Operand Operand2 { get; set; }
    }

    public class ArithmeticInstruction : BinaryInstruction {}
    
    public class LogicalInstruction: BinaryInstruction {}
}