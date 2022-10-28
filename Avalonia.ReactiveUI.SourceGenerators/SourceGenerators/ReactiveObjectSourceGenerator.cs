using Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions;
using Avalonia.ReactiveUI.SourceGenerators.Generation.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using ReactiveUI;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Avalonia.ReactiveUI.SourceGenerators.Generation.SourceGenerators;

[Generator]
internal class ReactiveObjectSourceGenerator : ISourceGenerator
{
    private const string _templateName = "Templates.ReactiveObjectTemplate.txt";
    private static readonly string? _assemblyName = Assembly.GetAssembly(typeof(ReactiveObjectSourceGenerator)).GetName().Name;

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<ReactiveObjectAttribute> syntaxReceiver)
            return;

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(classSyntax);

            var attribute = classSyntax.AttributeLists.SelectMany(sm => sm.Attributes)
                .First(x => x.Name.ToString()
                                  .EnsureEndsWith("Attribute")
                                  .Equals(nameof(ReactiveObjectAttribute)));

            var sourceCode = GetSourceCodeFor(symbol as INamedTypeSymbol);
            context.AddSource($"{symbol?.Name}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() =>
            new AttributeSyntaxReceiver<ReactiveObjectAttribute>());
    }

    private string GetEmbededResource(string path)
    {
        using var stream = GetType().Assembly.GetManifestResourceStream(path);

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }

    private string? GetNamespaceRecursively(INamespaceSymbol? symbol)
    {
        if (symbol?.ContainingNamespace == null)
        {
            return symbol?.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }

    private string GetSourceCodeFor(INamedTypeSymbol? symbol)
    {
        var template = Template.Parse(GetEmbededResource($"{_assemblyName}.{_templateName}"));

        var reactiveMethods = symbol?.GetMembers()
                                     .OfType<IMethodSymbol>()
                                     .Where(x => x.GetAttributes()
                                                  .Any(a => a.AttributeClass?.Name?.EnsureEndsWith("Attribute")
                                                                                   .Equals(nameof(ReactiveCommandAttribute)) ?? false))
                                     ?? Array.Empty<IMethodSymbol>();

        var t = reactiveMethods.Count();

        var templateParameters = new ReactiveObjectTemplateParameters(symbol?.Name,
                                                                      symbol?.BaseType?.Name,
                                                                      symbol?.BaseType?.ContainingNamespace,
                                                                      symbol?.ContainingNamespace,
                                                                      reactiveMethods);

        return template.Render(new
        {
            Data = templateParameters
        }, new MemberRenamerDelegate(x => x.Name.ToPascalCase()));
    }
}

public class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver
   where TAttribute : Attribute
{
    public IList<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.AttributeLists.Count > 0 &&
            classDeclarationSyntax.AttributeLists
                .Any(al => al.Attributes
                    .Any(a => a.Name.ToString()
                                    .EnsureEndsWith("Attribute")
                                    .Equals(typeof(TAttribute).Name))))
        {
            Classes.Add(classDeclarationSyntax);
        }
    }
}