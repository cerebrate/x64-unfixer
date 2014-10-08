using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Arkane.KSP.X64Unfixer
{
    class Program
    {
        static void Main (string[] args)
        {
            // We expect one parameter.
            if (args.Length != 1)
            {
                Console.WriteLine ("usage: x64-unfixer <filename>");
                return;
            }

            var filepath = args[0];
            string oldpath = String.Concat (Path.GetDirectoryName (filepath), Path.DirectorySeparatorChar, Path.GetFileNameWithoutExtension (filepath), ".old");

            // So now we hack.
            // Load in the assembly.
            AssemblyDefinition assembly;

            try
            {
                assembly = AssemblyDefinition.ReadAssembly (filepath, new ReaderParameters (ReadingMode.Immediate));
            }
            catch (Exception ex)
            {
                Console.WriteLine ("Error loading assembly: {0} - is it a valid .NET .dll file?", ex.Message);
                return;
            }

            Console.WriteLine ("Assembly loaded: {0}...", assembly.Name);

            // See, I could just have disabled that one function and everything would have been swell.
            // But you weren't going to let me do that, were you?
            // So now I have to do this brute-force crap instead.
            // For science. You monster.

            try
            {

                ModuleDefinition corlib = ModuleDefinition.ReadModule (typeof (object).Module.FullyQualifiedName);
                TypeDefinition intDefinition = corlib.GetType ("System.Int32");
                TypeDefinition intptrDefinition = corlib.GetType ("System.IntPtr");
                PropertyDefinition sizeDefinition = intptrDefinition.Properties.Single (pd => pd.Name == "Size");
                MethodDefinition getDefinition = sizeDefinition.GetMethod;

                // Iterate through every single type in the module.
                foreach (var td in assembly.MainModule.Types)
                {
                    Console.WriteLine ("Examining type: {0}", td.Name);

                    bool updatedMethod = false;

                    // Iterate through every single method in the type.
                    // This includes constructors, property implementors, etc.
                    foreach (var md in td.Methods)
                    {
                        Console.WriteLine ("  Examining method: {0}", md.Name);

                        // Examine their IL.
                        var ilp = md.Body.GetILProcessor ();

                        List<Instruction> toReplace = new List<Instruction> ();

                        foreach (Instruction fe in ilp.Body.Instructions)
                        {
                            if ((fe.OpCode == OpCodes.Call) &&
                                (fe.Operand != null) &&
                                (((MethodReference) fe.Operand).Name == "get_Size"))
                            {
                                Console.WriteLine ("    System.IntPtr reference found, replacing...");

                                toReplace.Add(fe);

                                assembly.MainModule.Import (md);
                                updatedMethod = true;
                            }
                        }

                        foreach (Instruction fe in toReplace)
                        {
                            ilp.Replace (fe, Instruction.Create (OpCodes.Ldc_I4_4));
                        }
                    }

                    // If we updated the method, we must also update the type.
                    if (updatedMethod)
                        assembly.MainModule.Import (td);
                }

                // So sorry, cleverer than that.

                File.Move (filepath, oldpath);

                assembly.Write (filepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine ("Error rewriting assembly: {0}", ex.Message);
            }

            Console.WriteLine ("Operation completed.");

            Console.ReadLine ();
        }
    }
}
