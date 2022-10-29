using Avalonia.ReactiveUI.SourceGenerators.Generation.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avalonia.ReactiveUI.SourceGenerators.Templates.Base;

internal class BaseTemplateParameters
{
    protected string? parentClassName;
    protected string? parentNamespace;

    public BaseTemplateParameters(
        string? typeName,
        string? parentClassName,
        INamespaceSymbol? parentNamespace,
        INamespaceSymbol? rootNamespace)
    {
        ClassName = typeName;
        ParentClassName = parentClassName;
        ParentNamespace = GetNamespaceRecursively(parentNamespace);
        Namespace = GetNamespaceRecursively(rootNamespace);
    }

    public string? ClassName { get; set; }
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

    protected virtual string? GetNamespaceRecursively(INamespaceSymbol? symbol)
    {
        if (symbol?.ContainingNamespace == null)
        {
            return symbol?.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }
}
