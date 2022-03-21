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
        internal IEnumerable<IPropertySymbol> Properties => Symbol.GetProperties();
        private IEnumerable<IPropertySymbol> KeyProperties => Symbol.GetKeyProperties();
        internal IEnumerable<IPropertySymbol> PrimitiveProperties => Properties.Where(x => x.IsBasicKind());
        internal IEnumerable<IPropertySymbol> GenericProperties => Properties.Where(x 
                                                => x.Type.IsGenericType()
                                                && x.Type.Name != "Nullable");
        public IEnumerable<PropertyModel> ViewProperties => Properties.Except(GenericProperties)
                                .Select(x => new PropertyModel(x));

        public IEnumerable<PropertyModel> LookupProperties => ViewProperties
                                .Where(p => PrimitiveProperties.Any(x => x.Name.Equals($"{p.Name}Id", StringComparison.OrdinalIgnoreCase)));

        public IEnumerable<PropertyModel> DisplayProperties => ViewProperties.Where(x => x.Name != "Id"
                                                    && x.Name != "CreatedOn"
                                                    && x.Name != "CreatedBy"
                                                    && x.Name != "UpdatedOn"
                                                    && x.Name != "UpdatedBy");

        public IEnumerable<PropertyModel> OwnedProperties => DisplayProperties.Except(LookupProperties)
                                .Where(p => !PrimitiveProperties.Any(x => x.Name.Equals(p.Name)));

        public string PropertyVariables => string.Join(", ",
                                    DisplayProperties.Select(x => x.VariableName));

        public EntityModel(INamedTypeSymbol symbol) => Symbol = symbol;
    }
}