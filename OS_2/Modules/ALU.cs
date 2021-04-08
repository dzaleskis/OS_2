using System;
using OS_2.Concepts;
using OS_2.Exceptions;
using OS_2.Utils;

namespace OS_2.Modules
{
    [Flags]
    public enum Flags
    {
        ZF = 0x01,
        SF = 0x02,
        OF = 0x04,
        CF = 0x08
    }

    public class ALU
    {
        public Flags Flags { get; set; }

        private void SetFlags(UnaryInstruction instruction, int result)
        {
            // reset flags
            Flags &= 0;

            if (result == 0)
            {
                Flags |= Flags.ZF;
            }

            if (result < 0)
            {
                Flags |= Flags.SF;
            }
        }

        private void SetFlags(BinaryInstruction instruction, int result)
        {
            // reset flags
            Flags &= 0;
            
            if (result == 0)
            {
                Flags |= Flags.ZF;
            }
            
            if (result < 0)
            {
                Flags |= Flags.SF;
            }
            
            // need to limit to 16 bits
            bool ALastBitSet = instruction.A.ToUshort().IsLastBitSet();
            bool BLastBitSet = instruction.B.ToUshort().IsLastBitSet();
            bool ResLastBitSet = result.ToUshort().IsLastBitSet();
            
            // set the carry & overflow flags here
            switch (instruction.Opcode)
            {
                case Opcode.ADD:
                    if ((ALastBitSet || BLastBitSet) && !ResLastBitSet)
                    {
                        Flags |= Flags.CF;
                    }
                    if ((!ALastBitSet && !BLastBitSet) && ResLastBitSet)
                    {
                        Flags |= Flags.OF;
                    }
                    if ((ALastBitSet && BLastBitSet) && !ResLastBitSet)
                    {
                        Flags |= Flags.OF;
                    }
                    break;
                case Opcode.SUB:
                    if ((!ALastBitSet && !BLastBitSet) && ResLastBitSet)
                    {
                        Flags |= Flags.CF;
                    }
                    if ((!ALastBitSet && BLastBitSet) && ResLastBitSet)
                    {
                        Flags |= Flags.OF;
                    }
                    if ((ALastBitSet && !BLastBitSet) && !ResLastBitSet)
                    {
                        Flags |= Flags.OF;
                    }
                    break;
            }
        }

        public int Process(BinaryInstruction instruction)
        {
            int result = 0;
            
            switch (instruction.Opcode)
            {
                case Opcode.ADD:
                    result = (short)(instruction.A.ToShort() + instruction.B.ToShort());
                    break;
                case Opcode.SUB:
                    result = (short)(instruction.A.ToShort() - instruction.B.ToShort());
                    break;
                case Opcode.MUL:
                    result = (ushort)(instruction.A.ToUshort() * instruction.B.ToUshort());
                    break;
                case Opcode.IMUL:
                    result = (short)(instruction.A.ToShort() * instruction.B.ToShort());
                    break;
                case Opcode.DIV:
                    result = (ushort)(instruction.A.ToUshort() / instruction.B.ToUshort());
                    break;
                case Opcode.IDIV:
                    result = (short)(instruction.A.ToShort() / instruction.B.ToShort());
                    break;
                case Opcode.AND:
                    result = (ushort)(instruction.A.ToUshort() & instruction.B.ToUshort());
                    break;
                case Opcode.OR:
                    result = (ushort)(instruction.A.ToUshort() | instruction.B.ToUshort());
                    break;
                case Opcode.XOR:
                    result = (ushort)(instruction.A.ToUshort() ^ instruction.B.ToUshort());
                    break;
                default:
                    throw new OperationException("Unrecognized opcode");
            }
            
            SetFlags(instruction, result);
            return result;
        }
        
        
        public int Process(UnaryInstruction instruction)
        {
            int result = 0;
            
            switch (instruction.Opcode)
            {
                case Opcode.NOT:
                    result = (ushort)(~instruction.A.ToUshort());
                    break;
                default:
                    throw new OperationException("Unrecognized opcode");
            }
            
            SetFlags(instruction, result);
            return result;
        }
        
    }
}