using System.Linq;
using System.Text.RegularExpressions;
using Humanizer;
using Microsoft.CodeAnalysis;
using Spawn.Models;

namespace Spawn.Extensions
{
    public static class IPropertySymbolExtensions
    {
        public static bool IsBasicKind(this IPropertySymbol symbol)
        {
            return symbol.Type.IsBasicKind();
        }

        public static bool IsLongString(this IPropertySymbol symbol)
        {
            return symbol.GetAttributes().Any(x => "MaxLength" == x.AttributeClass?.Name
                                                || ("StringLength" == x.AttributeClass?.Name
                                                && x.ConstructorArguments.Any(y => 255 <= (int?)y.Value)));
        }

        public static string PropertyPath(this IPropertySymbol symbol)
        {
            if (symbol.Type.IsValueType || symbol.Type.Name == "System.String")
                return symbol.Name;
            return $"{symbol.Name}.{symbol.PropertyPath()}";
        }

        public static string DisplayName(this IPropertySymbol symbol)
        {
            var attribute = symbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "DisplayAttribute");
            var name = (string?)attribute?.ConstructorArguments.FirstOrDefault().Value;
            if (string.IsNullOrWhiteSpace(name))
                return symbol.NameAsWords();
            return name;
        }

        public static string DisplayPath(this IPropertySymbol symbol)
        {
            if (symbol.IsBasicKind())
                return symbol.Name;

            var childProperties = (symbol.Type as INamedTypeSymbol)?.GetDisplayProperties();
            if (childProperties.Any())
            {
                if (childProperties.Count() > 1)
                    return childProperties.Select(x => $"{symbol.Name}.{x.Name}").Aggregate((x, y) => $"{x},{y}");
                return $"{symbol.Name}.{childProperties.Single().Name}";
            }

            return symbol.Name;
        }

        public static string NameAsWords(this IPropertySymbol symbol) => Regex.Replace(symbol.Name, "([A-Z])", " $1", RegexOptions.Compiled).Trim();

    }
}