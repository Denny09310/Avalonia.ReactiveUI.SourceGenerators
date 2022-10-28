using Avalonia.ReactiveUI.SourceGenerators.Contracts;

namespace Avalonia.ReactiveUI.SourceGenerators.Models.Base;

internal class BaseReactiveCommandDeclaration : IReactiveCommandParts
{
    public const string UnitTypeName = "Unit";

    public BaseReactiveCommandDeclaration(string? tParam, string? tResult, string? commandName)
    {
        TResult = tResult;
        TParam = tParam;
        CommandName = commandName;
    }

    public string? CommandName { get; set; }
    public string? TParam { get; set; }
    public string? TResult { get; set; }
}