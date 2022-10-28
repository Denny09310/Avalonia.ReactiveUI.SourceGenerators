using Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions;
using Avalonia.ReactiveUI.SourceGenerators.Models.Base;
using System.Linq;

namespace Avalonia.ReactiveUI.SourceGenerators.Models;

internal class PublicReactiveCommandDeclaration : BaseReactiveCommandDeclaration
{
    public PublicReactiveCommandDeclaration(
        string? tParam,
        string? tResult,
        string? commandName,
        PrivateReactiveCommandDeclaration privateReactiveCommand) : base(tParam, tResult, commandName)
    {
        PrivateReactiveCommand = privateReactiveCommand;
    }

    public string? CanExecute { get; set; }
    public bool IsTask { get; set; }
    public string? MethodName { get; set; }
    public PrivateReactiveCommandDeclaration PrivateReactiveCommand { get; set; }

    public override string ToString()
    {
        return $"public ReactiveCommand<{TParam},{TResult}> {CommandName?.ToPascalCase()} " +
               $"=> {PrivateReactiveCommand.CommandName} ??= ReactiveCommand.{GetCommandFactoryMethod()};";
    }

    private string GetCommandFactoryMethod()
    {
        var types = new[] { IsUnit(TResult), IsUnit(TParam) }.Where(x => x != null);

        string genericTypes = types.Any() ? $"<{string.Join(",", types)}>" : string.Empty;
        string methodCallback = CanExecute is { } ? $"{MethodName}, {CanExecute}()" : $"{MethodName}";
        string factoryType = IsTask ? "CreateFromTask" : "Create";

        return $"{factoryType}{genericTypes}({methodCallback})";
    }

    private string? IsUnit(string? value) => value == UnitTypeName ? null : value;
}