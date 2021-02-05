

using System;

namespace Cw7.Exceptions
{
    public class EnrollmentException : Exception
    {
        public EnrollmentException()
        {
        }

        public EnrollmentException(string message) : base(message)
        {
        }
    }
}
