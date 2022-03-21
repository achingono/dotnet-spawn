using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Spawn.Extensions
{
    public static class IDictionaryExtensions
    {
        public static INamedTypeSymbol FindClass(this IDictionary<Project, Compilation> compilations, string classFullName)
        {
            return compilations.SelectMany(x => x.Value.GetSymbolsWithName(y =>
                                    y == classFullName.Substring(classFullName.LastIndexOf('.') + 1)))
                                .OfType<INamedTypeSymbol>()
                                .Single(x =>
                                    x.ContainingNamespace.Name == classFullName.Substring(0, classFullName.LastIndexOf('.')));
        }

        public static IEnumerable<INamedTypeSymbol> FindDbContexts(this IDictionary<Project, Compilation> compilations)
        {
            return compilations.SelectMany(x => x.Value.GetNamedTypeSymbols())
                                .Where(x => x.BaseType?.Name == "DbContext")
                                .Distinct();
        }

        public static IEnumerable<ISymbol> GetSymbolsWithName(this IDictionary<Project, Compilation> compilations, string name, SymbolFilter filter = SymbolFilter.TypeAndMember)
        {
            return compilations.Values.SelectMany(x => x.GetSymbolsWithName(name, filter));
        }

        public static IEnumerable<INamedTypeSymbol> GetNamedTypeSymbols(this IDictionary<Project, Compilation> compilations)
        {
            return compilations.SelectMany(x => x.Value.GetNamedTypeSymbols());
        }
    }
}