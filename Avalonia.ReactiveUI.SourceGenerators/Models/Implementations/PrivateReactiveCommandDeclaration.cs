using Avalonia.ReactiveUI.SourceGenerators.Models.Base;

namespace Avalonia.ReactiveUI.SourceGenerators.Models.Implementations;

internal class PrivateReactiveCommandDeclaration : BaseReactiveCommandDeclaration
{
    public PrivateReactiveCommandDeclaration(string? tParam, string? tResult, string? commandName) : base(tParam, tResult, commandName)
    {
    }

    public override string ToString()
    {
        return $"private ReactiveCommand<{TParam},{TResult}> {CommandName};";
    }
}