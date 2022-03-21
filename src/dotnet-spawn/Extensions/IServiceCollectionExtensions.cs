using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RazorLight;
using Spawn.Generators;
using Spawn.Models;

namespace Spawn.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, 
            FileInfo solution,
            Func<INamedTypeSymbol, bool> predicate,
            Func<TemplateModel, string> fileNameGenerator,
            Func<FileInfo> templateAccessor
            )
        {
            return services.AddLogging(configure => configure.AddConsole())
                            .AddMSBuildWorkspace()
                            .AddSolution(solution.FullName)
                            .AddCompilations()
                            .AddScoped(services => predicate)
                            .AddScoped(services => fileNameGenerator)
                            .AddScoped(services => templateAccessor)
                            .AddRazorEngine()
                            .AddGenerators();
        }

        public static IServiceCollection AddMSBuildWorkspace(this IServiceCollection services)
        {
            return services.AddScoped<MSBuildWorkspace>(services =>
            {
                MSBuildLocator.RegisterDefaults();
                var workspace = MSBuildWorkspace.Create();
                var logger = services.GetService<ILogger<MSBuildWorkspace>>();
                workspace.WorkspaceFailed += (sender, workspaceFailedArgs) => logger.LogDebug(workspaceFailedArgs.Diagnostic.Message);

                return workspace;
            });
        }

        public static IServiceCollection AddRazorEngine(this IServiceCollection services)
        {
            return services.AddScoped<IRazorLightEngine>(services => new RazorLightEngineBuilder()
                .UseFileSystemProject(services.GetService<Func<FileInfo>>()().DirectoryName)
                .UseMemoryCachingProvider()
                .Build()
            );
        }

        public static IServiceCollection AddSolution(this IServiceCollection services, string solutionPath)
        {
            return services.AddScoped<Solution>(services =>
            {
                return services.GetService<MSBuildWorkspace>()
                   .OpenSolutionAsync(solutionPath)
                   .Result;
            });
        }

        public static IServiceCollection AddCompilations(this IServiceCollection services)
        {
            return services.AddScoped<IDictionary<Project, Compilation>>(services =>
                services.GetService<Solution>()
                        .Projects
                        .Select(x => (Project: x, Compilation: x.GetCompilationAsync().Result))
                        .ToDictionary(x => x.Project, y => y.Compilation)
            );
        }

        public static IServiceCollection AddProject(this IServiceCollection services, string projectName)
        {
            return services.AddScoped<Project>(services => services.GetService<Solution>()
                                .Projects
                                .Single(x => x.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase)));
        }

        public static IServiceCollection AddGenerators(this IServiceCollection services)
        {
            return services.AddScoped<IGenerator, MultipleFileGenerator>()
                            .AddScoped<IGenerator, SingleFileGenerator>();
        }
    }
}