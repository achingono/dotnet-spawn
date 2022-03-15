using System.CommandLine;
using System.IO;

namespace Spawn;

public class CommandArguments
{
    public IConsole Console { get; set; } = default!;

    public FileInfo Config { get; set; } = default!;

    public FileInfo Project { get; set; } = default!;

    public string Namespace { get; set; } = default!;

    public Verbosity Verbosity { get; set; }

    public bool Force { get; set; } = false;
}