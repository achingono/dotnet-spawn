using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Spawn.Models;

namespace Spawn;

public static partial class StandardOptions
{
    public static Option<Func<TemplateModel, string>> Pattern { get; } =
        new Option<Func<TemplateModel, string>>(
            new[] { "-p", "--pattern" }, 
            parseArgument: TryParseExpression,
            description: "Specify the lambda expression used for the nanes of the generated code file(s)"
        ){ 
            IsRequired = false,
            Arity = ArgumentArity.ExactlyOne 
        };

    private static Func<TemplateModel, string> TryParseExpression(ArgumentResult result)
    {
        var token = result.Tokens.Count switch
        {
            0 => "model => $\"{model.Name}.cs\"",
            1 => result.Tokens[0].Value,
            _ => throw new InvalidOperationException("Unexpected token count."),
        };

        var options = ScriptOptions.Default
                                    .AddReferences(
                                        MetadataReference.CreateFromFile (typeof (string).Assembly.Location),
                                        MetadataReference.CreateFromFile (typeof(TemplateModel).Assembly.Location),
                                        MetadataReference.CreateFromFile (typeof(Func<TemplateModel, string>).Assembly.Location)
                                    );
        var script = CSharpScript.Create<Func<TemplateModel, string>>(token, options);
        var state = script.RunAsync().Result;
        return state.ReturnValue;
    }
}