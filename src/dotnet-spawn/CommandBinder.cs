using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace Spawn;

public class CommandBinder : BinderBase<CommandArguments>
{
    protected override CommandArguments GetBoundValue(BindingContext bindingContext)
    {
        return new CommandArguments
        {
            Project = bindingContext.ParseResult.GetValueForOption(StandardOptions.Project),
            Template = bindingContext.ParseResult.GetValueForOption(StandardOptions.Template),
            Generator = bindingContext.ParseResult.GetValueForOption(StandardOptions.Generator),
            @Namespace = bindingContext.ParseResult.GetValueForOption(StandardOptions.Namespace),
            Output = bindingContext.ParseResult.GetValueForOption(StandardOptions.Output),
            Pattern = bindingContext.ParseResult.GetValueForOption(StandardOptions.Pattern) ?? 
                        (model => $"{model.Name}.cs"),
            Match = bindingContext.ParseResult.GetValueForOption(StandardOptions.Match) ??
                        (symbol => symbol.IsReferenceType && !(symbol.IsAbstract || symbol.IsNamespace || symbol.IsVirtual)),
            Verbosity = bindingContext.ParseResult.GetValueForOption(StandardOptions.Verbosity),
            Force = bindingContext.ParseResult.GetValueForOption(StandardOptions.Force)
        };
    }
}