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
    public static Option<Func<INamedTypeSymbol, bool>> Match { get; } =
        new Option<Func<INamedTypeSymbol, bool>>(
            new[] { "-m", "--match" }, 
            parseArgument: TryParsePredicate,
            description: "Specify the lambda expression used for the nanes of the generated code file(s)"
        ){ 
            IsRequired = false,
            Arity = ArgumentArity.ExactlyOne 
        };

    private static Func<INamedTypeSymbol, bool> TryParsePredicate(ArgumentResult result)
    {
        var token = result.Tokens.Count switch
        {
            0 => "symbol => symbol.IsReferenceType && !(symbol.IsAbstract || symbol.IsNamespace || symbol.IsVirtual)",
            1 => result.Tokens[0].Value,
            _ => throw new InvalidOperationException("Unexpected token count."),
        };


        var options = ScriptOptions.Default
                                    .AddReferences(
                                        MetadataReference.CreateFromFile (typeof (bool).Assembly.Location),
                                        MetadataReference.CreateFromFile (typeof (INamedTypeSymbol).Assembly.Location),
                                        MetadataReference.CreateFromFile (typeof(Func<INamedTypeSymbol, bool>).Assembly.Location)
                                    );
        var script = CSharpScript.Create<Func<INamedTypeSymbol, bool>>(token, options);
        var state = script.RunAsync().Result;
        return state.ReturnValue;
    }
}