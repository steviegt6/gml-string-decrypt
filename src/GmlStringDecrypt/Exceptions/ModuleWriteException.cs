using System;
using System.Runtime.Serialization;

namespace GmlStringDecrypt.Exceptions
{
    [Serializable]
    public class ModuleWriteException : Exception
    {
        public ModuleWriteException() { }
        public ModuleWriteException(string message) : base(message) { }
        public ModuleWriteException(string message, Exception inner) : base(message, inner) { }

        protected ModuleWriteException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context) { }
    }
}