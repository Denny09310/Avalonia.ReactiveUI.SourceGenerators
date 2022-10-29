using Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions;
using Avalonia.ReactiveUI.SourceGenerators.Generation.Models;
using Avalonia.ReactiveUI.SourceGenerators.Templates.Base;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.ReactiveUI.SourceGenerators.Generation.Templates;

internal class ReactiveObjectTemplateParameters : BaseTemplateParameters
{
    public ReactiveObjectTemplateParameters(
        string? typeName,
        string? parentClassName,
        INamespaceSymbol? parentNamespace,
        INamespaceSymbol? rootNamespace,
        IEnumerable<IMethodSymbol> reactiveMethods)
            : base(typeName, parentClassName, parentNamespace, rootNamespace)
    {
        CommandsDeclarations = GetCommandsDeclaration(reactiveMethods);
    }

    public IEnumerable<ReactiveCommandParts> CommandsDeclarations { get; set; }

    private IEnumerable<ReactiveCommandParts> GetCommandsDeclaration(IEnumerable<IMethodSymbol> reactiveMethods)
    {
        return reactiveMethods.Select(x => x.ParseReactiveCommand());
    }
}