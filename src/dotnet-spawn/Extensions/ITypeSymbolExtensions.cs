using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace Spawn.Extensions
{
    public static class ITypeSymbolExtensions
    {
        /// <remarks>
        /// TypedConstant isn't computing its own kind from the type symbol because it doesn't
        /// have a way to recognize the well-known type System.Type.
        /// </remarks>
        public static TypedConstantKind GetTypedConstantKind(this ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Byte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Decimal:
                case SpecialType.System_Char:
                case SpecialType.System_String:
                case SpecialType.System_Object:
                case SpecialType.System_DateTime:
                    return TypedConstantKind.Primitive;
                default:
                    if (type.Name == "Guid")
                        return TypedConstantKind.Primitive;
                    switch (type.TypeKind)
                    {
                        case TypeKind.Array:
                            return TypedConstantKind.Array;
                        case TypeKind.Enum:
                            return TypedConstantKind.Enum;
                        case TypeKind.Error:
                            return TypedConstantKind.Error;
                    }
                    return TypedConstantKind.Error;
            }
        }

        public static bool IsBasicKind(this ITypeSymbol symbol)
        {
            var kind = symbol.GetTypedConstantKind();
            return kind == TypedConstantKind.Primitive
                    || kind == TypedConstantKind.Array
                    || kind == TypedConstantKind.Enum
                    || (symbol.Name == "Nullable"
                    && (symbol as INamedTypeSymbol)!.TypeArguments.Single().IsBasicKind());
        }

        public static bool IsGenericType(this ITypeSymbol symbol)
        {
            var namedSymbol = symbol as INamedTypeSymbol;
            return namedSymbol != null
                    && (namedSymbol.IsGenericType
                    || (namedSymbol.ConstructedFrom != null
                    && namedSymbol.ConstructedFrom.IsGenericType));
        }
    }
}