using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C969_Task_1
{
    public class UnknownUserException : Exception
    {
        public UnknownUserException()
        {
        }

        public UnknownUserException(string message)
            : base(message)
        {
        }

        public UnknownUserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class ValidationException : Exception
    {
        public ValidationException()
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class DuplicateRecordException : Exception
    {
        public DuplicateRecordException()
        {
        }

        public DuplicateRecordException(string message)
            : base(message)
        {
        }

        public DuplicateRecordException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
