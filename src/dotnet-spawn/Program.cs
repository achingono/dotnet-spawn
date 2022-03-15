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
        var command = new RootCommand()
        {
            Description = "Developer tools and publishing for microservices.",
        };
        command.AddGlobalOption(StandardOptions.Config);
        command.AddGlobalOption(StandardOptions.Project);
        command.AddGlobalOption(StandardOptions.Namespace);
        command.AddGlobalOption(StandardOptions.Verbosity);
        command.AddGlobalOption(StandardOptions.Force);

        // Show commandline help unless a subcommand was used.
        command.Handler = CommandHandler.Create<IHelpBuilder>(help =>
        {
            help.Write(command);
            return 1;
        });

        return await command.InvokeAsync(args);
    }
}