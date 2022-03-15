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
            TreatUnmatchedTokensAsErrors = false
        };
        command.AddGlobalOption(StandardOptions.Config);
        command.AddGlobalOption(StandardOptions.Project);
        command.AddGlobalOption(StandardOptions.Namespace);
        command.AddGlobalOption(StandardOptions.Verbosity);
        command.AddGlobalOption(StandardOptions.Force);


        // Show commandline help unless a subcommand was used.
        command.Handler = CommandHandler.Create<CommandArguments, ParseResult, IHelpBuilder, IConsole>(async (arguments, result, help, console) =>
        {
            if (result.UnmatchedTokens.Count <= 0)
            {
                help.Write(command);
                return 1;
            }

            foreach(var token in result.UnmatchedTokens)
                console.Out.WriteLine($"Executing command: '{token}'.");
            return 0;
        });

        return await command.InvokeAsync(args);
    }
}