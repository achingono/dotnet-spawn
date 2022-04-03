using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Spawn.Extensions;
using Spawn.Generators;
using Spawn.Models;

namespace Spawn;

static partial class Program
{
    public static async Task<int> Main(string[] args)
    {
        var parser = BuildParser();

        return await parser.InvokeAsync(args);
    }

    private static Parser BuildParser()
    {

        var command = new RootCommand()
        {
            Description = "Roslyn based code generator.",
            TreatUnmatchedTokensAsErrors = true
        };

        command.AddGlobalOption(StandardOptions.Project);
        command.AddGlobalOption(StandardOptions.Template);
        command.AddGlobalOption(StandardOptions.Generator);
        command.AddGlobalOption(StandardOptions.Namespace);
        command.AddGlobalOption(StandardOptions.Match);
        command.AddGlobalOption(StandardOptions.Pattern);
        command.AddGlobalOption(StandardOptions.Output);
        command.AddGlobalOption(StandardOptions.Verbosity);
        command.AddGlobalOption(StandardOptions.Force);

        command.SetHandler<CommandArguments, IConsole>(Handle, new CommandBinder());

        var builder = new CommandLineBuilder(command);
        return builder.UseDefaults().Build();
    }

    private static async void Handle(CommandArguments args, IConsole console)
    {
        if (args.Verbosity != Verbosity.Quiet)
            console.Out.WriteLine($"Loading project: '{args.Project}'.");

        try
        {
            using (var serviceProvider = new ServiceCollection()
                                        .AddServices(
                                            args.Project,
                                            args.Match,
                                            args.Pattern,
                                            () => args.Template,
                                            args.Verbosity
                                        )
                                        .BuildServiceProvider())
            {
                if (args.Verbosity != Verbosity.Quiet)
                    console.Out.WriteLine($"Loading generator: '{args.Generator}'.");

                var generators = serviceProvider.GetServices<IGenerator>()
                                                .Where(g => g.Name.Equals(args.Generator));


                foreach (var generator in generators)
                {
                    if (args.Verbosity != Verbosity.Quiet)
                        console.Out.WriteLine($"Executing generator: '{generator.Name}'.");
                        
                    await generator.GenerateAsync(args.Namespace, args.Output.FullName);
                }
            }
        }
        catch (Exception ex)
        {
            console.Error.WriteLine(ex.Message);
            if (args.Verbosity == Verbosity.Debug)
                console.Error.WriteLine(ex.StackTrace!);
        }
    }
}
