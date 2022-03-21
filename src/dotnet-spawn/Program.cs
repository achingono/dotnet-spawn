using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Threading.Tasks;

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
        command.AddGlobalOption(StandardOptions.Output);
        command.AddGlobalOption(StandardOptions.Verbosity);
        command.AddGlobalOption(StandardOptions.Force);

        command.SetHandler<CommandArguments, IConsole>(Handle, new CommandBinder());

        var builder = new CommandLineBuilder(command);
        return builder.UseDefaults().Build();
    }

    private static void Handle(CommandArguments args, IConsole console)
    {
        console.Out.WriteLine($"Executing generator: '{args.Generator}'.");
        console.Out.WriteLine($"Loading project: '{args.Project}'.");
    }
}