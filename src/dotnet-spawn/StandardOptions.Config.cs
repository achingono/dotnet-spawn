using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;

namespace Spawn;

public static partial class StandardOptions
{
    public static Option Config
    {
        get
        {
            var argument = new Argument<FileInfo>(TryParse, isDefault: true)
            {
                Arity = ArgumentArity.ZeroOrOne,
                Name = "configuration file or directory",
            };

            return new Option(new[] { "-c", "--config", "--configuration" }, "Configuration file or directory")
            {
                Argument = argument,
            };

            static bool TryFindConfigFile(string directoryPath, out string? configFilePath, out string? errorMessage)
            {
                var matches = new List<string>();
                foreach (var candidate in Directory.EnumerateFiles(directoryPath))
                {
                    if (Path.GetExtension(candidate).EndsWith(".json"))
                    {
                        matches.Add(candidate);
                    }
                }

                // Prefer spawn.json if multiple json files are in the same directory 
                if (matches.Any(m => m.EndsWith("spawn.json")))
                {
                    matches.RemoveAll(m => !m.EndsWith("spawn.json"));
                }

                if (matches.Count == 0)
                {
                    errorMessage = $"No config file or solution file was found in directory '{directoryPath}'.";
                    configFilePath = default;
                    return false;
                }
                else if (matches.Count == 1)
                {
                    errorMessage = null;
                    configFilePath = matches[0];
                    return true;
                }
                else
                {
                    errorMessage = $"More than one config file or solution file was found in directory '{directoryPath}'.";
                    configFilePath = default;
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
                    if (TryFindConfigFile(token, out var filePath, out var errorMessage))
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