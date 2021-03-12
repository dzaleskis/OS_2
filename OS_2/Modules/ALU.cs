using System;
using OS_2.Concepts;
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
        private Flags _flags;

        private void SetFlags(UnaryInstruction instruction, int result)
        {
            // reset flags
            _flags &= 0;

            if (result == 0)
            {
                _flags |= Flags.ZF;
            }

            if (result < 0)
            {
                _flags |= Flags.SF;
            }
        }

        private void SetFlags(BinaryInstruction instruction, int result)
        {
            // reset flags
            _flags &= 0;
            
            if (result == 0)
            {
                _flags |= Flags.ZF;
            }
            
            if (result < 0)
            {
                _flags |= Flags.SF;
            }
            
            // need to limit to 16 bits
            bool ALastBitSet = instruction.A.ToUshort().IsLastBitSet();
            bool BLastBitSet = instruction.B.ToUshort().IsLastBitSet();
            bool ResLastBitSet = result.ToUshort().IsLastBitSet();
            
            // set the carry flag here
            switch (instruction.Opcode)
            {
                case Opcode.ADD:
                case Opcode.IADD:
                    if ((ALastBitSet || BLastBitSet) && !ResLastBitSet)
                    {
                        _flags |= Flags.CF;
                    }
                    if ((!ALastBitSet && !BLastBitSet) && ResLastBitSet)
                    {
                        _flags |= Flags.OF;
                    }
                    if ((ALastBitSet && BLastBitSet) && !ResLastBitSet)
                    {
                        _flags |= Flags.OF;
                    }
                    break;
                case Opcode.SUB:
                case Opcode.ISUB:
                    if ((!ALastBitSet && !BLastBitSet) && ResLastBitSet)
                    {
                        _flags |= Flags.CF;
                    }
                    if ((!ALastBitSet && BLastBitSet) && ResLastBitSet)
                    {
                        _flags |= Flags.OF;
                    }
                    if ((ALastBitSet && !BLastBitSet) && !ResLastBitSet)
                    {
                        _flags |= Flags.OF;
                    }
                    break;
            }
        }

        public Flags GetFlags()
        {
            return _flags;
        }

        public int Process(BinaryInstruction instruction)
        {
            int result = 0;
            
            switch (instruction.Opcode)
            {
                case Opcode.ADD:
                    result = (ushort)(instruction.A.ToUshort() + instruction.B.ToUshort());
                    break;
                case Opcode.IADD:
                    result = (short)(instruction.A.ToShort() + instruction.B.ToShort());
                    break;
                case Opcode.SUB:
                    result = (ushort)(instruction.A.ToUshort() - instruction.B.ToUshort());
                    break;
                case Opcode.ISUB:
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
                    throw new Exception("Unrecognized opcode");
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
                    throw new Exception("Unrecognized opcode");
            }
            
            SetFlags(instruction, result);
            return result;
        }
        
    }
}