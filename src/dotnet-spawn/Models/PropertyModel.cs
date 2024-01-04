using System;
using System.Collections;
using Humanizer;
using Microsoft.CodeAnalysis;
using Spawn.Extensions;

namespace Spawn.Models
{
    public class PropertyModel
    {
        public string Name => Symbol.Name;
        public string TypeName => Symbol.Type.Name;
        public string VariableName => $"{Name.Substring(0, 1).ToLowerInvariant()}{Name.Substring(1)}";
        public string Filter => $"{{{VariableName}:{TypeName.ToLowerInvariant()}}}";
        public string CollectionName => (Name.EndsWith("Id") ? Label : TypeName).Pluralize();
        public string CollectionVariableName => $"{CollectionName.Substring(0, 1).ToLowerInvariant()}{CollectionName.Substring(1)}";
        public string Label => Name.EndsWith("Id") ? Name.Substring(0, Name.Length - 2) : Name;
        public bool IsLongString => Symbol.IsLongString();
        public bool IsBasicKind => Symbol.Type.IsBasicKind();
        public bool IsVirtual => Symbol.IsVirtual;
        public IPropertySymbol Symbol { get; }
        public EntityModel EntityModel { get; }
        public SpecialType SpecialType => Symbol.Type.SpecialType;
        public TypeKind TypeKind => Symbol.Type.TypeKind;
        public TypedConstantKind TypedConstantKind => Symbol.Type.GetTypedConstantKind();

        public PropertyModel(IPropertySymbol symbol)
        {
            Symbol = symbol;
            EntityModel = new EntityModel(symbol.Type as INamedTypeSymbol);
        }

        public override bool Equals(object other)
        {
            // Reference equality check
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            // Adjust for your type...
            PropertyModel otherModel = (PropertyModel)other;

            // You may want to change the equality used here based on the
            // types of Equipment and Destiny
            return this.Name.Equals(otherModel.Name)
                && this.TypeName.Equals(otherModel.TypeName);
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + Name.GetHashCode();
            hash = hash * 31 + TypeName.GetHashCode();
            return hash;
        }
    }
}