using System.CommandLine;
using System.IO;
using Microsoft.CodeAnalysis;
using Spawn.Models;

namespace Spawn;

public class CommandArguments
{
    public FileInfo Project { get; set; } = default!;

    public FileInfo Template { get; set; } = default!;

    public string Generator { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public FileSystemInfo Output { get; set; } = default!;
    public Func<TemplateModel, string> Pattern { get; set; } = default!;
    public Func<INamedTypeSymbol, bool> Match { get; set; } = default!;
    public Verbosity Verbosity { get; set; }

    public bool Force { get; set; } = false;

}