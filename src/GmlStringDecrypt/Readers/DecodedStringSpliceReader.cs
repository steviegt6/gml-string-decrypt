using System.Linq;
using GmlStringDecrypt.Util;
using Mono.Cecil;
using MonoMod.Cil;

namespace GmlStringDecrypt.Readers
{
    public class DecodedStringSpliceReader : IILReader<DecodedStringSpliceReader.DecodedStringSplice?>
    {
        public readonly record struct DecodedStringSplice(MethodDefinition Method, int CacheAccessIndex, int CacheWriteIndex, int StartPosition, int SpliceLength);

        public DecodedStringSplice? Read(ILCursor c) {
            // Expected IL pattern:
            //  ldsfld string[] [cacheArray]
            //  ldc.i4 [cacheAccessIndex]
            //  ldelem.ref
            //  dup
            //  brtue.s [ret label]  ────────╮
            //                               │
            //  pop                          │
            //  ldc.i4 [cacheWriteIndex]     │
            //  ldc.i4 [startPosition]       │
            //  ldc.i4 [spliceLength]        │
            //  call string [decoderMethod (string,string,string)]
            //                               │
            //  ret  <───────────────────────╯
            
            // If the 4 ldc.i4 opcodes don't exist, we aren't looking for this method.
            if (c.Instrs.Count(x => x.IsLdcI4()) != 4) return null;

            int[] values = new int[4];
            for (int i = 0; i < values.Length; i++) {
                int value = 0;
                c.GotoNext(x => x.MatchLdcI4(out value));
                
                values[i] = value;
            }
            
            return new DecodedStringSplice(c.Method, values[0], values[1], values[2], values[3]);
        }
    }
}