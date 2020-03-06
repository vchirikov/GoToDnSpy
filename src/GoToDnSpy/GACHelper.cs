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
        private readonly static Lazy<Dictionary<string, string>> _gacNetframework2;
        private readonly static Lazy<Dictionary<string, string>> _gacNetframework4;

        static GacHelper()
        {
            _gacFolders = new string[] { "GAC", "GAC_32", "GAC_64", "GAC_MSIL" };
            _systemrootPath = Environment.GetEnvironmentVariable("systemroot") ?? throw new GoToDnSpyException($"Enviroment variable 'systemroot' doesn't exists!");
            _referenceAssemblyPath_x64 = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles") ?? @"c:\Program Files\", "Reference Assemblies");
            _referenceAssemblyPath_x86 = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? @"c:\Program Files (x86)\", "Reference Assemblies");

            // map assemblyName -> file path
            _gacNetframework2 = new Lazy<Dictionary<string, string>>(() => ReadGacAssemblyNames(Path.Combine(_systemrootPath, "assembly")));
            _gacNetframework4 = new Lazy<Dictionary<string, string>>(() => ReadGacAssemblyNames(Path.Combine(_systemrootPath, "Microsoft.NET", "assembly")));
        }

        private static Dictionary<string, string> ReadGacAssemblyNames(string gacRoot)
        {
            var result = new Dictionary<string, string>(512, StringComparer.Ordinal);
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
                    catch { }

                    if (assemblyName == null)
                        continue;
                    result[assemblyName.FullName] = assemblyFile;
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
        public static string FindAssemblyInGac(string assemblyFullName)
        {
            if (_gacNetframework4.Value.TryGetValue(assemblyFullName, out var result))
                return result;

            if (_gacNetframework2.Value.TryGetValue(assemblyFullName, out result))
                return result;

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
