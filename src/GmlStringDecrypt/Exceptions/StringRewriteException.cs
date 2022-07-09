using System;
using System.Runtime.Serialization;

namespace GmlStringDecrypt.Exceptions
{
    [Serializable]
    public class StringRewriteException : Exception
    {
        public StringRewriteException() { }
        public StringRewriteException(string message) : base(message) { }
        public StringRewriteException(string message, Exception inner) : base(message, inner) { }

        protected StringRewriteException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context) { }
    }
}