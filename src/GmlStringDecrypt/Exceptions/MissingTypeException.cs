using System;

namespace GmlStringDecrypt.Exceptions
{
    public class MissingTypeException : Exception
    {
        public MissingTypeException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
    }
}