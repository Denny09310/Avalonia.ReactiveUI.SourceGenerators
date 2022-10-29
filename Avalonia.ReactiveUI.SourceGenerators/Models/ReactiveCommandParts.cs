using Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions;
using Avalonia.ReactiveUI.SourceGenerators.Models.Implementations;

namespace Avalonia.ReactiveUI.SourceGenerators.Generation.Models;

internal class ReactiveCommandParts
{
    public ReactiveCommandParts(string? tResult, string? tParam, string commandName)
    {
        string privateCommandName = $"_{commandName.ToCamelCase()}";

        PrivateCommandDeclaration = new(tResult, tParam, privateCommandName);
        PublicCommandDeclaration = new(tResult, tParam, commandName, PrivateCommandDeclaration);
    }

    public string? CanExecute { get; set; }
    public bool IsTask { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public PrivateReactiveCommandDeclaration? PrivateCommandDeclaration { get; private set; }
    public PublicReactiveCommandDeclaration PublicCommandDeclaration { get; private set; }

    public ReactiveCommandParts SetPublicDeclaration()
    {
        PublicCommandDeclaration.IsTask = IsTask;
        PublicCommandDeclaration.MethodName = MethodName;
        PublicCommandDeclaration.CanExecute = CanExecute;

        return this;
    }
}