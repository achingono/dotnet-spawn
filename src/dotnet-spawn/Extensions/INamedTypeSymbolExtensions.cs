using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Spawn.Extensions
{
    public static class INamedTypeSymbolExtensions
    {
        public static IEnumerable<IPropertySymbol> GetProperties(this INamedTypeSymbol symbol)
        {
            return symbol.GetMembers()
                        .Where(x => x.Kind == SymbolKind.Property)
                        .OfType<IPropertySymbol>();
        }

        public static IEnumerable<IPropertySymbol> GetGenericProperties(this INamedTypeSymbol symbol)
        {
            return symbol.GetProperties()
                        .Where(x => x.Type.IsGenericType());
        }

        public static IEnumerable<IPropertySymbol> GetDbSetProperties(this INamedTypeSymbol symbol)
        {
            return symbol.GetGenericProperties()
                        .Where(x => x.Type.Name == "DbSet");
        }

        public static IEnumerable<IPropertySymbol> GetKeyProperties(this INamedTypeSymbol symbol)
        {
            var properties = symbol.GetProperties();
            var primaryKeys = properties.Where(x => x.GetAttributes().Any(y => "KeyAttribute" == y.AttributeClass?.Name)
                                            || x.Name == "Id");

            if (primaryKeys.Any()) return primaryKeys;

            primaryKeys = Regex.Matches(symbol.Name, "(^[a-z]+|[A-Z]+(?![a-z])|[A-Z][a-z]+)")
                                .OfType<Match>()
                                .Select(m => properties.SingleOrDefault(p => p.Name == $"{m.Value}Id"))!;

            return primaryKeys;
        }

        public static IEnumerable<IPropertySymbol> GetVirtualProperties(this INamedTypeSymbol symbol)
        {
            return symbol.GetProperties()
                .Where(x => x.IsVirtual);
        }

        public static IEnumerable<IPropertySymbol> GetPatchableProperties(this INamedTypeSymbol symbol)
        {
            var properties = symbol.GetProperties();
            var primaryKeys = symbol.GetKeyProperties();

            return properties.Except(primaryKeys)
                .Where(x => !x.IsVirtual);
        }

        public static IEnumerable<IPropertySymbol> GetDisplayProperties(this INamedTypeSymbol symbol)
        {
            var attribute = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "DisplayColumnAttribute");
            var propertyName = (string?)attribute?.ConstructorArguments.FirstOrDefault().Value;
            if (string.IsNullOrWhiteSpace(propertyName))
                yield break;
            if (propertyName.Contains(','))
            {
                var propertyNames = propertyName.Split(",", StringSplitOptions.RemoveEmptyEntries);
                foreach (var property in symbol.GetProperties().Where(x => propertyNames.Contains(x.Name)))
                    yield return property;
            }
            foreach( var property in symbol.GetProperties().Where(x => x.Name == propertyName))
                yield return property;
        }
    }
}