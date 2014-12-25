#region header

// x64-unfixer - Program.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2013.  All rights reserved.
// 
// Created: 2014-10-10 6:05 PM

#endregion

#region using

using System ;
using System.Collections.Generic ;
using System.IO ;

using Mono.Cecil ;
using Mono.Cecil.Cil ;

#endregion

namespace Arkane.KSP.X64Unfixer
{
    internal static class Program
    {
        private static void Main (string[] args)
        {
            // We expect one parameter.
            if (args.Length != 1)
            {
                Console.WriteLine ("usage: x64-unfixer <filename>") ;
                return ;
            }

            var filepath = args[0] ;
            var oldpath = String.Concat (Path.GetDirectoryName (filepath),
                                         Path.DirectorySeparatorChar,
                                         Path.GetFileNameWithoutExtension (filepath),
                                         ".old") ;

            // So now we hack.

            string kspDir = FindKsp () ;

            // Add the KSP subdirectory to the resolver base path.
            string assemblyDir = Path.Combine (kspDir, @"KSP_x64_Data\Managed");
            var resolver = new DefaultAssemblyResolver () ;
            resolver.AddSearchDirectory(assemblyDir);

            // Load in the assembly.
            AssemblyDefinition assembly ;

            try
            {
                var rParam = new ReaderParameters (ReadingMode.Immediate) {AssemblyResolver = resolver} ;

                assembly = AssemblyDefinition.ReadAssembly (filepath, rParam) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine ("Error loading assembly: {0} - is it a valid .NET .dll file?", ex.Message) ;
                return ;
            }

            Console.WriteLine ("Assembly loaded: {0}...", assembly.Name) ;

            // See, I could just have disabled that one function and everything would have been swell.
            // But you weren't going to let me do that, were you?
            // So now I have to do this brute-force crap instead.
            // For science. You monster.

            try
            {
                var updatedType = false ;

                // Iterate through every single type in the module.
                foreach (var td in assembly.MainModule.Types)
                {
                    Console.WriteLine ("Examining type: {0}", td.Name) ;

                    var updatedMethod = false ;

                    if (!td.IsClass)
                        continue ;

                    // Iterate through every single method in the type.
                    // This includes constructors, property implementors, etc.
                    foreach (var md in td.Methods)
                    {
                        Console.WriteLine ("  Examining method: {0}", md.Name) ;

                        if (!md.HasBody)
                            continue ;

                        // Examine their IL.
                        var ilp = md.Body.GetILProcessor () ;

                        var toReplace = new List<Instruction> () ;

                        foreach (var fe in ilp.Body.Instructions)
                        {
                            if ((fe.OpCode == OpCodes.Call) &&
                                (fe.Operand != null) &&
                                (((MethodReference) fe.Operand).Name == "get_Size") &&
                                (((MethodReference) fe.Operand).DeclaringType != null) &&
                                (((MethodReference) fe.Operand).DeclaringType.FullName == "System.IntPtr")
                                )
                            {
                                Console.WriteLine ("    System.IntPtr reference found, replacing...") ;

                                toReplace.Add (fe) ;

                                assembly.MainModule.Import (md) ;
                                updatedMethod = true ;
                            }
                        }

                        foreach (var fe in toReplace)
                            ilp.Replace (fe, Instruction.Create (OpCodes.Ldc_I4_4)) ;
                    }

                    // If we updated the method, we must also update the type.
                    if (updatedMethod)
                    {
                        assembly.MainModule.Import (td) ;
                        updatedType = true ;
                    }
                }

                // So sorry, cleverer than that.

                if (updatedType)
                {
                    File.Move (filepath, oldpath) ;
                    Console.WriteLine ("Saved backup of unmodified assembly as: {0}", oldpath) ;

                    assembly.Write (filepath) ;
                    Console.WriteLine ("Wrote cracked assembly as: {0}", filepath) ;
                }
                else
                    Console.WriteLine ("Nothing to change.") ;
            }
            catch (Exception ex)
            {
                Console.WriteLine ("Error rewriting assembly: {0}", ex.Message) ;
                return ;
            }

            Console.WriteLine ("Operation completed.") ;
        }

        public static string FindKsp ()
        {
            if ((Environment.OSVersion.Platform != PlatformID.Win32NT) ||
                (!Environment.Is64BitOperatingSystem))
            {
                Console.WriteLine("Sorry, I only run on Windows x64 platforms.");
                Console.WriteLine ("Why would you need me for non-win64 anyway?");
                Environment.Exit(1);
            }

            string kspDir = null;

            var uninstallWowKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey (@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

            if (uninstallWowKey == null)
                throw new InvalidOperationException("FATALITY: Cannot find add/remove programs key.");

            foreach (var kn in uninstallWowKey.GetSubKeyNames ())
            {
                var k = uninstallWowKey.OpenSubKey (kn) ;
                if (k.GetValue ("DisplayName") as string == "Kerbal Space Program")
                {
                    kspDir = k.GetValue ("InstallLocation") as string ;
                    break ;
                }
            }

            if (kspDir == null)
            {
                Console.WriteLine("FATALITY: KSP not installed or could not find install location.");
                Environment.Exit(2);
            }

            Console.WriteLine ("KSP found at: {0}", kspDir);
          
            return kspDir;
        } 
    }
}
