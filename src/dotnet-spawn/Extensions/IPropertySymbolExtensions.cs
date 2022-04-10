using System.Linq;
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
    }
}