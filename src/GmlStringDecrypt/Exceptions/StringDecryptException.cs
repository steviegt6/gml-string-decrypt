using System;
using System.Runtime.Serialization;

namespace GmlStringDecrypt.Exceptions
{
    [Serializable]
    public class StringDecryptException : Exception
    {
        public StringDecryptException() { }
        public StringDecryptException(string message) : base(message) { }
        public StringDecryptException(string message, Exception inner) : base(message, inner) { }

        protected StringDecryptException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context) { }
    }
}