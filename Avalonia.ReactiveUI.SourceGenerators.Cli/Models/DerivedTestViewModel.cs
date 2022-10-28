using Avalonia.ReactiveUI.SourceGenerators.Cli.Models.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Avalonia.ReactiveUI.SourceGenerators.Cli.Models;

[ReactiveObject]
public partial class DerivedTestViewModel : ViewModelBase
{
    [Reactive]
    public string RandomString { get; set; } = string.Empty;

    [ReactiveCommand]
    private void GenerateRandomString()
    {
        RandomString = "askjhdkjashkdja";
    }
}
