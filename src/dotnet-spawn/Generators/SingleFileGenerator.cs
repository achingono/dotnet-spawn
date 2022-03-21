using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using RazorLight;
using Spawn.Extensions;
using Spawn.Models;

namespace Spawn.Generators
{
    public class SingleFileGenerator : IGenerator
    {
        public string Name => "SingleFile";

        public IRazorLightEngine Engine { get; }

        public IDictionary<Project, Compilation> Compilations { get; }

        public ILogger<SingleFileGenerator> Logger { get; }

        private Func<INamedTypeSymbol, bool> SymbolPredicate { get; }

        private Func<FileInfo> TemplateAccessor { get; }

        public SingleFileGenerator(IRazorLightEngine engine,
            IDictionary<Project, Compilation> compilations,
            ILogger<SingleFileGenerator> logger,
            Func<INamedTypeSymbol, bool> symbolPredicate,
            Func<FileInfo> templateAccessor)
        {
            Engine = engine;
            Compilations = compilations;
            Logger = logger;
            SymbolPredicate = symbolPredicate;
            TemplateAccessor = templateAccessor;
        }

        public async Task GenerateAsync(string targetNamespace, string targetPath)
        {
            var entities = Compilations.GetNamedTypeSymbols()
                                    .Where(SymbolPredicate)
                                    .Distinct(SymbolEqualityComparer.Default);

            var models = entities.Select(entity => new TemplateModel(entity!, targetNamespace));
            string result = await Engine.CompileRenderAsync(TemplateAccessor().FullName, models);
            var parent = Directory.GetParent(targetPath);

            if (parent != null && !parent.Exists)
                parent.Create();

            File.WriteAllText(targetPath, result);
            Logger.LogInformation($"Generated file: '{targetPath}'");
        }
    }
}
