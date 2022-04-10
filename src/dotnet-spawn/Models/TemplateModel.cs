using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Humanizer;

namespace Spawn.Models
{
    public class TemplateModel
    {
        public string Namespace { get; }   
        public string CollectionName => Name.Pluralize();
        public string Name => Entity.Symbol.Name;
        public EntityModel Entity { get; }

        public TemplateModel(ISymbol symbol, string @namespace)
        {
            Entity = new EntityModel(symbol as INamedTypeSymbol);
            Namespace = @namespace;
        }
    }
}
