using System.CommandLine;
using Microsoft.CodeAnalysis;
using Spawn.Models;

namespace Spawn;

public static partial class StandardOptions
{
    public static Option<string> Namespace { get; } =
        new Option<string>(
            new[] { "-n", "--namespace" }, 
            "Specify the namespace for the generated code file(s)"
        ){ 
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne 
        };

    public static Option<FileInfo> Template { get; } =
        new Option<FileInfo>(
            new[] { "-t", "--template" }, 
            "Specify the template used for the generated code file(s)"
        ){ 
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne 
        };


    public static Option<string> Generator { get; } =
        new Option<string>(
            new[] { "-g", "--generator" }, 
            "Specify the generator used for the generated code file(s)"
        ){ 
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne 
        };

    public static Option<FileSystemInfo> Output { get; } =
        new Option<FileSystemInfo>(
            new[] { "-o", "--output" }, 
            () => new DirectoryInfo("."),
            "Specify the file or folder used for the generated code file(s)"
        ){ 
            IsRequired = true,
            Arity = ArgumentArity.ExactlyOne 
        };

    public static Option<Verbosity> Verbosity { get; } =
        new Option<Verbosity>(
            new[] { "-v", "--verbosity" },
            () => Spawn.Verbosity.Info, 
            "Output verbosity"
        );

    public static Option<bool> Force { get; } =
        new Option<bool>(
            new[] { "-f", "--force" },
            "Force execution of command"
        );
}