using System;
using System.Runtime.Serialization;

namespace GmlStringDecrypt.Exceptions
{
    [Serializable]
    public class MissingFieldException : Exception
    {
        public MissingFieldException() { }
        public MissingFieldException(string message) : base(message) { }
        public MissingFieldException(string message, Exception inner) : base(message, inner) { }

        protected MissingFieldException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context) { }
    }
}