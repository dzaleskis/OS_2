using System;

namespace OS_2.Exceptions
{
    public class OperationException: Exception
    {
        public OperationException()
        {
        }

        public OperationException(string message)
            : base(message)
        {
        }
    }
}