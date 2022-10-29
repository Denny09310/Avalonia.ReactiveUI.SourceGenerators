using Avalonia.ReactiveUI.SourceGenerators.Extensions;
using Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions;
using Avalonia.ReactiveUI.SourceGenerators.Generation.Templates;
using Avalonia.ReactiveUI.SourceGenerators.SyntaxReceivers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ReactiveUI;
using Scriban;
using Scriban.Runtime;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Avalonia.ReactiveUI.SourceGenerators.Generation.SourceGenerators;

[Generator]
internal class ReactiveObjectSourceGenerator : ISourceGenerator
{
    private const string _templateName = "ReactiveObjectTemplate.txt";

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<ReactiveObjectAttribute> syntaxReceiver)
            return;

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Get the symbol with the
            ISymbol? symbol = context.GetAttributeSymbol<ReactiveObjectAttribute>(classSyntax, out _);
            var sourceCode = GetSourceCodeFor(symbol as INamedTypeSymbol);
            context.AddSource($"{symbol?.Name}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() =>
            new AttributeSyntaxReceiver<ReactiveObjectAttribute>());
    }

    private string GetSourceCodeFor(INamedTypeSymbol? symbol)
    {
        var template = Template.Parse(this.GetEmbededResource(_templateName));

        var reactiveMethods = symbol?.GetMembers()
                                     .OfType<IMethodSymbol>()
                                     .Where(x => x.GetAttributes()
                                                  .Any(HasReactiveCommandAttribute))
                                     ?? Array.Empty<IMethodSymbol>();

        var t = reactiveMethods.Count();

        var templateParameters = new ReactiveObjectTemplateParameters(symbol?.Name,
                                                                      symbol?.BaseType?.Name,
                                                                      symbol?.BaseType?.ContainingNamespace,
                                                                      symbol?.ContainingNamespace,
                                                                      reactiveMethods);

        return template.Render(new { Data = templateParameters },
            new MemberRenamerDelegate(x => x.Name.ToPascalCase()));
    }

    private static bool HasReactiveCommandAttribute(AttributeData attribute)
    {
        return attribute.AttributeClass?.Name?.EnsureEndsWith("Attribute")
                                              .Equals(nameof(ReactiveCommandAttribute)) ?? false;
    }
}
