using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GoToDnSpy 
{
    static class GACHelper 
    {
        #region EnumerateGACAssemblies1_2

        private static string[] GACFolders1_2 = new string[] 
        {
            "GAC", "GAC_32", "GAC_64", "GAC_MSIL"
        };

        public static IEnumerable<Tuple<AssemblyName, String>> EnumerateGACAssemblies1_2()
        {
            foreach (var gacFolder in GACFolders1_2) 
            {
                var path = Path.Combine(Environment.GetEnvironmentVariable("systemroot"), "assembly", gacFolder);
                if (Directory.Exists(path)) 
                {
                    foreach (var item in EnumerateAssembliesFromFolderRecursive(path)) 
                    {
                        yield return item;
                    }
                }
            }
        }

        #endregion

        #region EnumerateGACAssemblies4

        private static string[] GACFolders4 = new string[]
        {
            "GAC_32", "GAC_64", "GAC_MSIL"
        };

        public static IEnumerable<Tuple<AssemblyName, String>> EnumerateGACAssemblies4() {
            foreach (var gacFolder in GACFolders4) 
            {
                var path = Path.Combine(Environment.GetEnvironmentVariable("systemroot"), "Microsoft.NET", "assembly", gacFolder);
                if (Directory.Exists(path)) 
                {
                    foreach (var item in EnumerateAssembliesFromFolderRecursive(path)) 
                    {
                        yield return item;
                    }
                }
            }
        }

        private static IEnumerable<Tuple<AssemblyName, String>> EnumerateAssembliesFromFolderRecursive(string path) {
            foreach (string assemblyFile in Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories)) 
            {
                AssemblyName assemblyName = null;
                try 
                {
                    assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
                }
                catch (Exception) {
                }

                if (assemblyName != null) 
                {
                    yield return Tuple.Create(assemblyName, assemblyFile);
                }
            }
        }

        #endregion

        #region FindAssemblyInGac

        public static Tuple<AssemblyName, string> FindAssemblyInGAC(AssemblyName assemblyName, int? clrVersion = null) 
        {
            if (clrVersion == null) 
            {
                return 
                    FindAssemblyInGAC(assemblyName, 4) ??
                    FindAssemblyInGAC(assemblyName, 2);
            }

            if (clrVersion < 4) 
            {
                return EnumerateGACAssemblies1_2().FirstOrDefault(x => x.Item1.FullName == assemblyName.FullName);
            }
            else 
            {
                return EnumerateGACAssemblies4().FirstOrDefault(x => x.Item1.FullName == assemblyName.FullName);
            }
        }

        #endregion


        private static Lazy<string> ReferenceAssemblyPath64 = new Lazy<string>(() => Environment.GetEnvironmentVariable("ProgramFiles"));
        private static Lazy<string> ReferenceAssemblyPath32 = new Lazy<string>(() => Environment.GetEnvironmentVariable("ProgramFiles(x86)"));
        public static bool IsReferenceAssembly(string path) {
           return path.StartsWith(ReferenceAssemblyPath64.Value) || path.StartsWith(ReferenceAssemblyPath32.Value);
        }
    }
}
