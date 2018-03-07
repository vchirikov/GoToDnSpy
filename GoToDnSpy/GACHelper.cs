using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GoToDnSpy
{
    internal static class GacHelper
    {

        private readonly static string _referenceAssemblyPath_x64;
        private readonly static string _referenceAssemblyPath_x86;
        private readonly static string _systemrootPath;
        private readonly static string[] _gacFolders;
        private readonly static Lazy<List<(AssemblyName AssemblyName, string Filepath)>> _gacNetframework2;
        private readonly static Lazy<List<(AssemblyName AssemblyName, string Filepath)>> _gacNetframework4;


        static GacHelper()
        {
            _gacFolders = new string[] { "GAC", "GAC_32", "GAC_64", "GAC_MSIL" };
            _systemrootPath = Environment.GetEnvironmentVariable("systemroot") ?? throw new GoToDnSpyException($"Enviroment variable 'systemroot' doesn't exists!");
            _referenceAssemblyPath_x64 = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles")      ?? @"c:\Program Files\",      "Reference Assemblies");
            _referenceAssemblyPath_x86 = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? @"c:\Program Files (x86)\","Reference Assemblies");

            _gacNetframework2 = new Lazy<List<(AssemblyName AssemblyName, string Filepath)>>(() => ReadGacAssemblyNames(Path.Combine(_systemrootPath, "assembly")));
            _gacNetframework4 = new Lazy<List<(AssemblyName AssemblyName, string Filepath)>>(() => ReadGacAssemblyNames(Path.Combine(_systemrootPath, "Microsoft.NET", "assembly")));
        }


        private static List<(AssemblyName AssemblyName, string Filepath)> ReadGacAssemblyNames(string gacRoot)
        {
            var result = new List<(AssemblyName AssemblyName, string Filepath)>();
            foreach (var gacFolder in _gacFolders)
            {
                var path = Path.Combine(gacRoot, gacFolder);
                if (!Directory.Exists(path))
                    continue;

                foreach (var assemblyFile in Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories))
                {
                    AssemblyName assemblyName = null;
                    try
                    {
                        assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
                    }
                    catch (Exception)
                    {
                    }

                    if (assemblyName == null)
                        continue;
                    result.Add((assemblyName, assemblyFile));
                }
            }
            return result;
        }

        /// <summary>
        /// Search assemblyName in GAC folders.
        /// First search in net framework 4, when search in gac net framework 2
        /// </summary>
        /// <param name="assemblyName">Assembly name for search</param>
        /// <returns>path to assembly or <c>null</c></returns>
        public static string FindAssemblyInGac(AssemblyName assemblyName)
        {
            // avoid linq alloccations
            foreach (var tuple in _gacNetframework4.Value)
            {
                if (string.Equals(tuple.AssemblyName.FullName, assemblyName.FullName, StringComparison.OrdinalIgnoreCase))
                    return tuple.Filepath;
            }
            foreach (var tuple in _gacNetframework2.Value)
            {
                if (string.Equals(tuple.AssemblyName.FullName, assemblyName.FullName, StringComparison.OrdinalIgnoreCase))
                    return tuple.Filepath;
            }
            return null;
        }

        /// <summary>
        /// Check that path in program files
        /// </summary>
        /// <param name="path">check path</param>
        /// <returns>true if in program files</returns>
        public static bool IsReferenceAssembly(string path) => path.StartsWith(_referenceAssemblyPath_x64, StringComparison.OrdinalIgnoreCase) || path.StartsWith(_referenceAssemblyPath_x86, StringComparison.OrdinalIgnoreCase);

    }
}
