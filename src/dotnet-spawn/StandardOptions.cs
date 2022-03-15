using System.CommandLine;

namespace Spawn;

public static partial class StandardOptions
{
    public static Option Verbosity =>
        new Option(new[] { "-v", "--verbosity" }, "Output verbosity")
        {
            Argument = new Argument<Verbosity>("one of: quiet|info|debug", () => Spawn.Verbosity.Info)
            {
                Arity = ArgumentArity.ExactlyOne,
            },
            Required = false,
        };

    public static Option Namespace =>
        new Option(new[] { "-n", "--namespace" })
        {
            Description = "Specify the namespace for the generated code files",
            Required = false,
            Argument = new Argument<string>(),
        };

    public static Option Force =>
        new Option(new[] { "-f", "--force" })
        {
            Argument = new Argument<bool>(),
            Description = "Force execution of command",
            Required = false
        };
}