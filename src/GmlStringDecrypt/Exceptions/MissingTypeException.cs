using System;
using System.Runtime.Serialization;

namespace GmlStringDecrypt.Exceptions
{
    [Serializable]
    public class MissingTypeException : Exception
    {
        public MissingTypeException() { }
        public MissingTypeException(string message) : base(message) { }
        public MissingTypeException(string message, Exception inner) : base(message, inner) { }

        protected MissingTypeException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context) { }
    }
}