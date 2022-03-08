using System;

namespace DataManagement
{
    public class NoEntryException : Exception
    {
        public NoEntryException()
        {
        }

        public NoEntryException(string message)
            : base(message)
        {
        }

        public NoEntryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    
    public class NoConfigurablesException : Exception
    {
        public NoConfigurablesException()
        {
        }

        public NoConfigurablesException(string message)
            : base(message)
        {
        }

        public NoConfigurablesException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    
    public class ClassNotRegisteredException : Exception
    {
        public ClassNotRegisteredException()
        {
        }

        public ClassNotRegisteredException(string message)
            : base(message)
        {
        }

        public ClassNotRegisteredException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    
    public class NoFieldException : Exception
    {
        public NoFieldException()
        {
        }

        public NoFieldException(string message)
            : base(message)
        {
        }

        public NoFieldException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}