using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;

namespace Avalonia.ReactiveUI.SourceGenerators.Cli.Models.Base;

[ReactiveObject]
public partial class ViewModelBase
{
    [Reactive] public int ReturnValue { get; set; }

    [ReactiveCommand]
    public int ClickButtonAndReturnInt()
    {
        return ReturnValue;
    }

    [ReactiveCommand]
    public Task<int> ClickButtonTaskAndReturnInt()
    {
        return Task.FromResult(ReturnValue);
    }

    [ReactiveCommand]
    private static void ClickButton()
    {
    }

    [ReactiveCommand]
    private static Task ClickButtonTask()
    {
        return Task.CompletedTask;
    }

    [ReactiveCommand]
    private static Task ClickButtonTaskWithParameter(int arg)
    {
        return Task.CompletedTask;
    }

    [ReactiveCommand]
    private static void ClickButtonWithParameter(int arg)
    {
    }

    [ReactiveCommand]
    private Task<int> ClickButtonTaskWithParameterAndReturnInt(int arg)
    {
        return Task.FromResult(ReturnValue);
    }

    [ReactiveCommand]
    private int ClickButtonWithParameterAndReturnInt(int arg)
    {
        return ReturnValue;
    }

    private IObservable<bool> CanExecuteObservable()
        => this.WhenAnyValue(x => x.ReturnValue)
               .Select(x => x > 1);

    [ReactiveCommand(CanExecute = nameof(CanExecuteObservable))]
    private void ClickButtonWithCanExecute()
    {

    }
}