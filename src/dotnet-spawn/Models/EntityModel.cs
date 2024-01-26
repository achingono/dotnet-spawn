using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Spawn.Extensions;

namespace Spawn.Models
{
    public class EntityModel
    {
        public INamedTypeSymbol Symbol { get; }
        public bool IsComplexType => Symbol.GetAttributes().Any(x => x.AttributeClass?.Name == "ComplexTypeAttribute");
        internal IEnumerable<IPropertySymbol> Properties => Symbol.GetProperties();
        public IEnumerable<PropertyModel> KeyProperties => Symbol.GetKeyProperties()
                                .Where(x => !string.IsNullOrWhiteSpace(x?.Name))
                                .Select(x => new PropertyModel(x));
        internal IEnumerable<IPropertySymbol> PrimitiveProperties => Properties.Where(x => x.IsBasicKind());
        internal IEnumerable<IPropertySymbol> GenericProperties => Properties.Where(x 
                                                => x.Type.IsGenericType()
                                                && x.Type.Name != "Nullable");
        public IEnumerable<PropertyModel> ViewProperties => Properties.Except(GenericProperties)
                                .Select(x => new PropertyModel(x));

        public IEnumerable<PropertyModel> LookupProperties => ViewProperties
                                .Where(p => PrimitiveProperties.Any(x => x.Name.Equals($"{p.Name}Id", StringComparison.OrdinalIgnoreCase)));

        public IEnumerable<PropertyModel> FormProperties => ViewProperties.Where(x => x.Name != "Id"
                                                    && x.Name != "CreatedOn"
                                                    && x.Name != "CreatedBy"
                                                    && x.Name != "UpdatedOn"
                                                    && x.Name != "UpdatedBy");
        public IEnumerable<PropertyModel> DisplayProperties => Symbol.GetDisplayProperties()
                                .Select(x => ViewProperties.SingleOrDefault(y => y.Name == x.Name) ?? new PropertyModel(x));

        public IEnumerable<PropertyModel> OwnedProperties => FormProperties.Except(LookupProperties)
                                .Where(p => !PrimitiveProperties.Any(x => x.Name.Equals(p.Name)));

        public string PropertyVariables => string.Join(", ",
                                    FormProperties.Select(x => x.VariableName));

        public string DisplayPropertyPath => DisplayProperties.Select(x => x.IsBasicKind ? x.Name : x.DisplayPath)
                                .Aggregate((x, y) => $"{x},{y}");
        public EntityModel(INamedTypeSymbol symbol) => Symbol = symbol;
    }
}