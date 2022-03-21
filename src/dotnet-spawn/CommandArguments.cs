using System.CommandLine;
using System.IO;

namespace Spawn;

public class CommandArguments
{
    public FileInfo Project { get; set; } = default!;

    public FileInfo Template { get; set; } = default!;

    public string Generator { get; set; } = default!;
    public string Namespace { get; set; } = default!;
    public FileSystemInfo Output { get; set; } = default!;

    public Verbosity Verbosity { get; set; }

    public bool Force { get; set; } = false;

}