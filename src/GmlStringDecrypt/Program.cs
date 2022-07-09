using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GmlStringDecrypt.Exceptions;
using GmlStringDecrypt.Readers;
using Mono.Cecil;

namespace GmlStringDecrypt
{
    public static class Program
    {
        // TODO: Config instead of this.
        public static bool PopValueInsteadOfErasingMethodBody = false;
        /*
         * 0 - input path
         * 1 - output path
         * 2 - fully qualified name to string crypto class
         */
        public static void Main(string[] args) {

            if (args.Length != 3) throw new ArgumentException("Provided arguments are not equal to three!");
            if (!File.Exists(args[0])) throw new FileNotFoundException("Input file not found!");
            Directory.CreateDirectory(Path.GetDirectoryName(args[1]) ?? throw new ArgumentException("Output path is not valid!"));
            
            ModuleDefinition def;
            StringDecrypt decrypt;
            StringDecrypt.DecryptData data;
            List<DecodedStringSpliceReader.DecodedStringSplice> spliceMethods;

            try {
                ReaderParameters readerParams = new()
                {
                    AssemblyResolver = new DefaultAssemblyResolver()
                };
                
                if (readerParams.AssemblyResolver is BaseAssemblyResolver baseResolver) {
                    baseResolver.AddSearchDirectory(Path.GetDirectoryName(args[0]));
                }
                
                def = ModuleDefinition.ReadModule(args[0], readerParams);
            }
            catch (Exception e) {
                throw new ModuleLoadException("An error occured whilst trying to read the module file!", e);
            }

            try {
                decrypt = new StringDecrypt(def, args[2]);
                data = decrypt.Decrypt();

                int columns = 15;
                int rows = (data.EncryptedBytes.Length / columns) + 1;
                // 3 characters per column, counting the space between columns. Two extra spaces to account for added readability padding.
                int nameLength = (columns * 3) + 2;
                string[] names = {"Encrypted bytes:", "Decrypted bytes:", "Decoded characters:"};

                for (int i = 0; i < names.Length; i++) {
                    int padding = nameLength - names[i].Length;
                    StringBuilder sb = new();
                    sb.Append(names[i]);
                    if (i != names.Length - 1) sb.Append(' ', padding);
                    Console.Write(sb.ToString());
                }

                Console.WriteLine();

                for (int row = 0; row < rows; row++) {
                    // Write encrypted bytes.
                    for (int column = 0; column < columns; column++) {
                        int index = row * columns + column;
                        
                        Console.Write(index >= data.EncryptedBytes.Length ? "  " : data.EncryptedBytes[index].ToString("X2"));
                        Console.Write(' ');
                        
                        if (column == columns - 1) Console.Write("| ");
                    }
                    
                    // Write decrypted bytes.
                    for (int column = 0; column < columns; column++) {
                        int index = row * columns + column;
                        
                        Console.Write(index >= data.DecryptedBytes.Length ? "  " : data.DecryptedBytes[index].ToString("X2"));
                        Console.Write(' ');
                        
                        if (column == columns - 1) Console.Write("| ");
                    }
                    
                    // Write decoded chars.
                    for (int column = 0; column < columns; column++) {
                        int index = row * columns + column;
                        
                        Console.Write(index >= data.DecodedCharacters.Length ? ' ' : data.DecodedCharacters[index]);
                        Console.Write(' ');
                        
                        if (column == columns - 1) Console.WriteLine('|');
                    }
                }
                
                Console.WriteLine("\nRaw decoded character output:");
                Console.WriteLine(data.DecodedCharacters);
            }
            catch (Exception e) {
                throw new StringDecryptException("An error occured whilst trying to decrypt!", e);
            }

            try {
                spliceMethods = decrypt.ResolveSpliceMethods().OrderBy(x => x.CacheAccessIndex).ToList();
                
                Console.WriteLine($"\nResolved {spliceMethods.Count} splice methods:");

                foreach (DecodedStringSpliceReader.DecodedStringSplice splice in spliceMethods) {
                    StringBuilder sb = new();
                    sb.Append($" [{splice.CacheAccessIndex}]");
                    sb.Append(' ', 3 - splice.CacheAccessIndex.ToString().Length);
                    sb.Append($" = {splice.StartPosition}");
                    sb.Append(' ', 4 - splice.StartPosition.ToString().Length);
                    sb.Append($" -> {splice.StartPosition + splice.SpliceLength}");
                    sb.Append(' ', 4 - (splice.StartPosition + splice.SpliceLength).ToString().Length);
                    sb.Append($" (+{splice.SpliceLength})");
                    Console.WriteLine(sb.ToString());
                }
            }
            catch (Exception e) {
                throw new ResolveStringSpliceException("An error occured whilst reading methods en masse for decoded string splicing!", e);
            }

            try {
                decrypt.Rewrite(data, spliceMethods);
                Console.WriteLine("\nRewrote methods.");
            }
            catch (Exception e) {
                throw new StringRewriteException("An error occured whilst trying to rewrite the type!", e);
            }

            try {
                def.Write(args[1], new WriterParameters()
                {
                    
                });
                Console.WriteLine($"\nWrote modified module to disk at: {args[1]}");
            }
            catch (Exception e) {
                throw new ModuleWriteException("An error occured whilst trying to write the modified module to disk!", e);
            }
            
        }
    }
}