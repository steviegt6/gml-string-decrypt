using System;
using System.Linq;
using System.Text;
using GmlStringDecrypt.Exceptions;
using Mono.Cecil;
using MonoMod.Cil;
using MissingFieldException = GmlStringDecrypt.Exceptions.MissingFieldException;

namespace GmlStringDecrypt
{
    public sealed class StringDecrypt
    {
        public readonly record struct DecryptData(byte[] EncryptedBytes, byte[] DecryptedBytes);

        private readonly ModuleDefinition Module;
        private readonly TypeDefinition DecryptType;
        private readonly MethodDefinition StaticConstructor;

        public StringDecrypt(ModuleDefinition module, string decryptClass) {
            Module = module;
            DecryptType = Module.GetType(decryptClass) ?? throw new MissingTypeException("Module did not contain type: " + decryptClass);
            StaticConstructor = DecryptType.Methods.FirstOrDefault(x => x.Name == ".cctor") ?? throw new MissingMethodException("Type did not contain .cctor: " + decryptClass);
        }

        public DecryptData Decrypt() {
            Console.WriteLine("Beginning decryption of strings in class: " + DecryptType.FullName);
            byte[] bytes = GetByteArray();
            Console.WriteLine();
            Console.WriteLine("Encrypted bytes length: " + bytes.Length);
            Console.WriteLine("Encrypted bytes content: " + string.Join(',',  bytes));
            uint or = GetOr();
            byte[] decryptedBytes = DecryptBytes(bytes, or);
            string chars = ConvertBytes(decryptedBytes);
            Console.WriteLine();
            Console.WriteLine("Logical exclusive OR: " + or);
            Console.WriteLine("Decrypted bytes length: " + decryptedBytes.Length);
            Console.WriteLine("Decrypted bytes content: " + string.Join(',',  decryptedBytes));
            Console.WriteLine("Readable character decoding: " + chars);
            
            return new DecryptData();
        }

        public void Rewrite(DecryptData data) {
        }

        private byte[] GetByteArray() {
            // Explanation:
            //  The actual bytes are initialized to a static value whose type comes from a nested struct in the decrypt class.
            //  The array value is just a bytearray that we can get from InitialValue.
            TypeDefinition structByteArray = DecryptType.NestedTypes.Single();
            FieldDefinition? field = DecryptType.Fields.FirstOrDefault(x => x.FieldType.FullName == structByteArray.FullName);

            if (field is null) throw new MissingFieldException("Could not find byte array field in class: " + DecryptType.FullName);
            
            return field.InitialValue;
        }

        private uint GetOr() {
            ILContext context = new(StaticConstructor);
            ILCursor cursor = new(context)
            {
                Next = null // Go to end of method.
            };

            // After jumpting to end, match to the nearest xor and then navigate to the ldc.i4 value that should be before it.
            // A natural (and real) example:
            // 	IL_003b: xor
            //  IL_003c: ldc.i4 170 <- Matching ldc.i4
            //  IL_0041: xor <- Matching first xor.
            //  ...
            //  <- our cursor starts here
            int or = 0;
            cursor.GotoPrev(x => x.MatchXor()).GotoPrev(x => x.MatchLdcI4(out or));

            return (uint) or;
        }

        private static byte[] DecryptBytes(byte[] bytes, uint or) {
            // TODO: Rewrite a section of the .cctor dynamically instead of doing this with hardcode.

            byte[] decryptedBytes = new byte[bytes.Length];
            
            for (int i = 0; i < bytes.Length; i++) {
                decryptedBytes[i] = (byte)((uint) (bytes[i] ^ i) ^ or);
            }

            return decryptedBytes;
        }

        // TODO: Use conversion method located in the class itself (usually '6').
        private static string ConvertBytes(byte[] decryptedBytes) {
            return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
        }
    }
}