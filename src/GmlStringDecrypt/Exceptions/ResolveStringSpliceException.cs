using System;
using System.Runtime.Serialization;

namespace GmlStringDecrypt.Exceptions
{
    [Serializable]
    public class ResolveStringSpliceException : Exception
    {
        public ResolveStringSpliceException() { }
        public ResolveStringSpliceException(string message) : base(message) { }
        public ResolveStringSpliceException(string message, Exception inner) : base(message, inner) { }

        protected ResolveStringSpliceException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context) { }
    }
}