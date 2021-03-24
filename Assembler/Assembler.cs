
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using OS_2.Concepts;
using OS_2.Utils;

namespace Assembler
{
    public class Assembler
    {
        private class ExtendedVariable : StaticVariable
        {
            public int Offset { get; set; }
        }

        private enum InstructionOperandType
        {
            None,
            Address,
            Immediate,
            Register
        }

        private static InstructionOperandType DetermineOperandType(Opcode opcode)
        {
            switch (opcode)
            {
                case Opcode.HALT:
                case Opcode.ADD:
                case Opcode.SUB:
                case Opcode.MUL:
                case Opcode.IMUL:
                case Opcode.DIV:
                case Opcode.IDIV:
                case Opcode.AND:
                case Opcode.OR:
                case Opcode.XOR:
                case Opcode.NOT:
                case Opcode.CMP:
                    return InstructionOperandType.None;
                case Opcode.LOD:
                    return InstructionOperandType.Address;
                case Opcode.LODE:
                    return InstructionOperandType.None;
                case Opcode.STO:
                    return InstructionOperandType.Address;
                case Opcode.STOE:
                    return InstructionOperandType.None;
                case Opcode.PUSH:
                    return InstructionOperandType.Immediate;
                case Opcode.POP:
                    return InstructionOperandType.None;
                case Opcode.JMP:
                case Opcode.JE:
                case Opcode.JL:
                case Opcode.JB:
                case Opcode.JLE:
                case Opcode.JBE:
                case Opcode.JG:
                case Opcode.JA:
                case Opcode.JGE:
                case Opcode.JAE:
                case Opcode.LOOP:
                case Opcode.CALL:
                    return InstructionOperandType.Address;
                case Opcode.RET:
                    return InstructionOperandType.None;
                case Opcode.INT:
                    return InstructionOperandType.Immediate;
                case Opcode.POPR:
                    return InstructionOperandType.Register;
                case Opcode.PUSHR:
                    return InstructionOperandType.Register;
                case Opcode.INB:
                    return InstructionOperandType.Immediate;
                case Opcode.OUTB:
                    return InstructionOperandType.Immediate;
                default:
                    throw new ArgumentException("invalid opcode");
            }
        }

        private static AssembledInstruction AssembleInstruction(NotAssembledInstruction instr, List<ExtendedVariable> extendedVariables)
        {
            var opcode = Enum.Parse<Opcode>(instr.Name);
            var operandType = DetermineOperandType(opcode);
            var parseSuccess = int.TryParse(instr.Operand, out int parsedOperand);
            int operand = 0;
                
            switch (operandType)
            {
                case InstructionOperandType.None:
                    break;
                    
                case InstructionOperandType.Address:
                    operand = parseSuccess
                        ? parsedOperand
                        : extendedVariables.Single(ev => ev.Name == instr.Operand).Offset;
                    break;
                    
                case InstructionOperandType.Immediate:
                    if (!parseSuccess)
                    {
                        throw new Exception("invalid immediate operand");
                    }

                    break;
                case InstructionOperandType.Register:
                    operand = (int) Enum.Parse<RealRegister>(instr.Operand);
                    break;
                default:
                    throw new Exception("unsupported instruction operand type");
            }

            return new AssembledInstruction()
            {
                Opcode = opcode,
                Operand = operand
            };
        }

        public static AssembledProgram Assemble(NotAssembledProgram notAssembledProgram)
        {
            // need to map all instructions that use variables to the corresponding values/addresses
          
            List<ExtendedVariable> extendedVariables =
                notAssembledProgram.Variables.Select((v, i) => new ExtendedVariable()
                {
                    Name = v.Name,
                    Offset = i * Constants.WORD_LENGTH,
                    Value = v.Value
                }).ToList();

            List<AssembledInstruction> assembledInstructions = notAssembledProgram.Instructions
                .Select(instr => AssembleInstruction(instr, extendedVariables)).ToList();
            
            var assembledVariables = extendedVariables.Select(ev => new AssembledVariable()
            {
                Value = ev.Value
            }).ToList();

            return new AssembledProgram()
            {
                AssembledVariables = assembledVariables,
                AssembledInstructions = assembledInstructions
            };
        }
    }
}