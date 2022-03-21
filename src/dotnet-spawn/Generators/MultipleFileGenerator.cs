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
    public class MultipleFileGenerator : IGenerator
    {
        public string Name => "MultipleFile";
        public IRazorLightEngine Engine { get; }

        public IDictionary<Project, Compilation> Compilations { get; }

        public ILogger<MultipleFileGenerator> Logger { get; }

        private Func<INamedTypeSymbol, bool> SymbolPredicate { get; }

        private Func<TemplateModel, string> FileNameGenerator { get; }

        private Func<FileInfo> TemplateAccessor { get; }

        public MultipleFileGenerator(IRazorLightEngine engine,
            IDictionary<Project, Compilation> compilations,
            ILogger<MultipleFileGenerator> logger,
            Func<INamedTypeSymbol, bool> symbolPredicate,
            Func<TemplateModel, string> fileNameGenerator,
            Func<FileInfo> templateAccessor)
        {
            Engine = engine;
            Compilations = compilations;
            Logger = logger;
            SymbolPredicate = symbolPredicate;
            FileNameGenerator = fileNameGenerator;
            TemplateAccessor = templateAccessor;
        }

        public async Task GenerateAsync(string targetNamespace, string targetPath)
        {
            var entities = Compilations.GetNamedTypeSymbols()
                                    .Where(SymbolPredicate)
                                    .Distinct(SymbolEqualityComparer.Default);

            foreach (var entity in entities)
            {
                var model = new TemplateModel(entity!, targetNamespace);
                var path = Path.Combine(targetPath, FileNameGenerator(model));
                string result = await Engine.CompileRenderAsync(TemplateAccessor().FullName, model);

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                File.WriteAllText(path, result);
                Logger.LogInformation($"Generated file: '{path}'");
            }
        }
    }
}
