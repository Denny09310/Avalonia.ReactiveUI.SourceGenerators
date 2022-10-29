using Avalonia.ReactiveUI.SourceGenerators.Contracts;

namespace Avalonia.ReactiveUI.SourceGenerators.Models.Base;

internal class BaseReactiveCommandDeclaration : IReactiveCommandDeclaration
{
    public const string UnitTypeName = "System.Reactive.Unit";

    public BaseReactiveCommandDeclaration(string? tParam, string? tResult, string? commandName)
    {
        TParam = tParam;
        TResult = tResult;
        CommandName = commandName;
    }

    public string? CommandName { get; set; }
    public string? TParam { get; set; }
    public string? TResult { get; set; }
}