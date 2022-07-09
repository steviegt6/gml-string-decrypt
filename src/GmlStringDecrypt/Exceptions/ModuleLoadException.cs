using System;
using System.Runtime.Serialization;

namespace GmlStringDecrypt.Exceptions
{
    [Serializable]
    public class ModuleLoadException : Exception
    {
        public ModuleLoadException() { }
        public ModuleLoadException(string message) : base(message) { }
        public ModuleLoadException(string message, Exception inner) : base(message, inner) { }

        protected ModuleLoadException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context) { }
    }
}