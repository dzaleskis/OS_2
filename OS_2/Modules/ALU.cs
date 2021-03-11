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
        
        private void SetFlags(int res, int a, int b)
        {
            // need a mechanism for defining what flags to set
            _flags = 0;
            
            // compute flag state here
            if (res == 0)
            {
                _flags |= Flags.ZF;
            }
            
            if (res < 0)
            {
                _flags |= Flags.SF;
            }

            if (res < 0 && a > 0 && b > 0)
            {
                _flags |= Flags.OF;
            } 
            else if (res > 0 && a < 0 && b < 0)
            {
                _flags |= Flags.OF;
            }

            if (res > short.MaxValue || a < b)
            {
                _flags |= Flags.CF;
            }
        }

        public Flags GetFlags()
        {
            return _flags;
        }

        // public InstructionResult Process(ArithmeticInstruction instruction)
        // {
        //     return new InstructionResult();
        // }
        //
        // public InstructionResult Process(LogicalInstruction instruction)
        // {
        //     return new InstructionResult();
        // }
    }
}