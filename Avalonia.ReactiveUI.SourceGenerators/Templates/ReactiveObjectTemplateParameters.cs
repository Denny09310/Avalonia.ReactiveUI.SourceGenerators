using Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions;
using Avalonia.ReactiveUI.SourceGenerators.Generation.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.ReactiveUI.SourceGenerators.Generation.Templates;

internal class ReactiveObjectTemplateParameters
{
    private string? parentClassName;
    private string? parentNamespace;

    public ReactiveObjectTemplateParameters(
        string? typeName,
        string? parentClassName,
        INamespaceSymbol? parentNamespace,
        INamespaceSymbol? rootNamespace,
        IEnumerable<IMethodSymbol> reactiveMethods)
    {
        ClassName = typeName;
        ParentClassName = parentClassName;
        ParentNamespace = GetNamespaceRecursively(parentNamespace);
        Namespace = GetNamespaceRecursively(rootNamespace);
        CommandsDeclarations = GetCommandsDeclaration(reactiveMethods);
    }

    public string? ClassName { get; set; }
    public IEnumerable<ReactiveCommandParts> CommandsDeclarations { get; set; }
    public string? Namespace { get; set; }

    public string? ParentClassName
    {
        get => parentClassName == nameof(Object)
            ? string.Empty
            : parentClassName;

        set => parentClassName = value;
    }

    public string? ParentNamespace
    {
        get => string.IsNullOrEmpty(parentClassName)
            ? string.Empty
            : parentNamespace == parentClassName
                ? string.Empty
                : parentNamespace;

        set => parentNamespace = value;
    }

    private IEnumerable<ReactiveCommandParts> GetCommandsDeclaration(IEnumerable<IMethodSymbol> reactiveMethods)
    {
        return reactiveMethods.Select(x => x.ParseCommand());
    }

    private string? GetNamespaceRecursively(INamespaceSymbol? symbol)
    {
        if (symbol?.ContainingNamespace == null)
        {
            return symbol?.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }
}