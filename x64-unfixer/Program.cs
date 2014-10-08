using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

            Console.WriteLine("Assembly loaded: {0}...", assembly.Name);

            // Find the CompatibilityChecker type.
            TypeDefinition cc;

            try
            {
                cc = assembly.MainModule.Types.Single (td => td.Name == "CompatibilityChecker");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not find CompatibilityChecker: {0}.", ex.Message);
                return;
            }

            Console.WriteLine("...found CompatibilityChecker...");

            // Find the IsWin64 method.
            MethodDefinition isW64;

            try
            {
                isW64 = cc.Methods.Single (md => md.Name == "IsWin64");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not find IsWin64() method: {0}", ex.Message);
                return;
            }    
            
            Console.WriteLine("...found IsWin64()...");

            // Patch it to always return false.
            try
            {
                var ilp = isW64.Body.GetILProcessor ();

                var first = ilp.Body.Instructions.First ();

                ilp.InsertBefore (first, ilp.Create(OpCodes.Ldc_I4_0));
                ilp.InsertBefore (first, ilp.Create (OpCodes.Ret));

                assembly.MainModule.Import (isW64);
                assembly.MainModule.Import (cc);

                File.Move(filepath, oldpath);

                assembly.Write(filepath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error rewriting assembly: {0}", ex.Message);
            }

            Console.WriteLine ("Operation completed.");
        }
    }
}
