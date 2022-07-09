using System.Linq;
using GmlStringDecrypt.Readers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace GmlStringDecrypt.Util
{
    public static class Extensions
    {
        public static readonly Code[] LdcI4Codes =
        {
            Code.Ldc_I4,
            Code.Ldc_I4_0,
            Code.Ldc_I4_1,
            Code.Ldc_I4_2,
            Code.Ldc_I4_3,
            Code.Ldc_I4_4,
            Code.Ldc_I4_5,
            Code.Ldc_I4_6,
            Code.Ldc_I4_7,
            Code.Ldc_I4_8,
            Code.Ldc_I4_M1,
            Code.Ldc_I4_S
        };
        
        public static bool IsLdcI4(this Instruction instr) {
            return LdcI4Codes.Contains(instr.OpCode.Code);
        }

        public static TData ReadMethod<TData>(this MethodDefinition method, IILReader<TData> reader) {
            return reader.Read(new ILCursor(new ILContext(method)));
        }
    }
}