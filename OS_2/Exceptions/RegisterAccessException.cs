using System;

namespace OS_2.Exceptions
{
    public class RegisterAccessException: Exception
    {
        public RegisterAccessException()
        {
        }

        public RegisterAccessException(string message)
            : base(message)
        {
        }
    }
}