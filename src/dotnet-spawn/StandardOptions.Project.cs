using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;

namespace Spawn;

public static partial class StandardOptions
{
    public static Option Project
    {
        get
        {
            var argument = new Argument<FileInfo>(TryParse, isDefault: true)
            {
                Arity = ArgumentArity.ZeroOrOne,
                Name = "project-file or solution-file or directory",
            };

            return new Option(new[] { "-p", "--project", }, "Project file, Solution file or directory")
            {
                Argument = argument,
            };

            static bool TryFindProjectFile(string directoryPath, out string? projectFilePath, out string? errorMessage)
            {
                var matches = new List<string>();
                foreach (var candidate in Directory.EnumerateFiles(directoryPath))
                {
                    if (Path.GetExtension(candidate).EndsWith(".sln"))
                    {
                        matches.Add(candidate);
                    }

                    if (Path.GetExtension(candidate).EndsWith(".csproj"))
                    {
                        matches.Add(candidate);
                    }
                }

                // Prefer solution if both are in the same directory. This helps
                // avoid some conflicts.
                if (matches.Any(m => m.EndsWith(".sln")))
                {
                    matches.RemoveAll(m => m.EndsWith(".csproj"));
                }

                if (matches.Count == 0)
                {
                    errorMessage = $"No project file or solution file was found in directory '{directoryPath}'.";
                    projectFilePath = default;
                    return false;
                }
                else if (matches.Count == 1)
                {
                    errorMessage = null;
                    projectFilePath = matches[0];
                    return true;
                }
                else
                {
                    errorMessage = $"More than one project file or solution file was found in directory '{directoryPath}'.";
                    projectFilePath = default;
                    return false;
                }
            }

            static FileInfo TryParse(ArgumentResult result)
            {
                var token = result.Tokens.Count switch
                {
                    0 => ".",
                    1 => result.Tokens[0].Value,
                    _ => throw new InvalidOperationException("Unexpected token count."),
                };

                if (File.Exists(token))
                {
                    return new FileInfo(token);
                }

                if (Directory.Exists(token))
                {
                    if (TryFindProjectFile(token, out var filePath, out var errorMessage))
                    {
                        return new FileInfo(filePath!);
                    }
                    else
                    {
                        result.ErrorMessage = errorMessage;
                        return default!;
                    }
                }

                result.ErrorMessage = $"The file '{token}' could not be found.";
                return default!;
            }
        }
    }
}