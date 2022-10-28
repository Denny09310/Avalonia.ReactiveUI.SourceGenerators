namespace Avalonia.ReactiveUI.SourceGenerators.Contracts;

internal interface IReactiveCommandParts
{
    public string? TParam { get; set; }
    public string? TResult { get; set; }
    public string? CommandName { get; set; }
}
