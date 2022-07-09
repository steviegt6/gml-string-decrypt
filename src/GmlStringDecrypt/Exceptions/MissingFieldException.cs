using System;

namespace GmlStringDecrypt.Exceptions
{
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
    }
}