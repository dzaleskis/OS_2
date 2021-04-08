using System;

namespace OS_2.Exceptions
{
    public class HaltException: Exception
    {
        public HaltException()
        {
        }

        public HaltException(string message)
            : base(message)
        {
        }
    }
}