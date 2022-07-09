using System;
using System.IO;
using GmlStringDecrypt.Exceptions;
using Mono.Cecil;
using MissingFieldException = GmlStringDecrypt.Exceptions.MissingFieldException;

namespace GmlStringDecrypt
{
    public static class Program
    {
        /*
         * 0 - input path
         * 1 - output path
         * 2 - fully qualified name to string crypto class
         */
        public static void Main(string[] args) {

            if (args.Length != 3) throw new ArgumentException("Provided arguments are not equal to three!");
            if (!File.Exists(args[0])) throw new FileNotFoundException("Input file not found!");
            Directory.CreateDirectory(Path.GetDirectoryName(args[1]) ?? throw new ArgumentException("Output path is not valid!"));
            
            ModuleDefinition def = null!;
            StringDecrypt decrypt = null!;
            StringDecrypt.DecryptData data = default;
            
            Safely(() => { def = ModuleDefinition.ReadModule(args[0]); })
               .CatchAll(e => throw new FileNotFoundException("Could not read input DLL file!", e))
               .Execute();

            Safely(() =>
                {
                     decrypt = new StringDecrypt(def, args[2]);
                     data = decrypt.Decrypt();
                })
               .Catch<MissingTypeException>(exception => throw new MissingTypeException("Could not find string crypto class!", exception))
               .Catch<MissingFieldException>(exception => throw new MissingFieldException("Could not find field!", exception))
               .Execute();

            Safely(() =>
                {
                    decrypt.Rewrite(data);
                })
               .Execute();
        }

        private static TryCatchFinally Safely(Action tryAction) {
            return new TryCatchFinally().Try(tryAction);
        }
    }
}