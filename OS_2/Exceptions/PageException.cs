using System;

namespace OS_2.Exceptions
{
    public class PageException: Exception
    {
        public PageException()
        {
        }

        public PageException(string message)
            : base(message)
        {
        }
    }
}