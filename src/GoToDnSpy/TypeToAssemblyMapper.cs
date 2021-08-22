using System.Collections.Generic;

namespace GoToDnSpy
{
    internal static class NamespaceToAssemblyMapper
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _map = Generated.CreateNamespaceTypeAssemblyMap();
        public static string? Get(string typeNamespace, string typeName)
            => (_map.TryGetValue(typeNamespace, out var typeAssemblyMap) && typeAssemblyMap.TryGetValue(typeName, out var result))
                ? result
                : null;

    }
}