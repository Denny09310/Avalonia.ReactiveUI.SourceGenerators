using Avalonia.ReactiveUI.SourceGenerators.Generation.Models;
using Avalonia.ReactiveUI.SourceGenerators.Models;
using Avalonia.ReactiveUI.SourceGenerators.Models.Base;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions
{
    internal static class SymbolExtensions
    {
        public static ISymbol? GetAttributeSymbol<T>(this GeneratorExecutionContext context, ClassDeclarationSyntax classSyntax, out AttributeSyntax attribute)
            where T : Attribute
        {
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(classSyntax);

            attribute = classSyntax.AttributeLists.SelectMany(sm => sm.Attributes)
                                                  .First(x => x.Name.ToString()
                                                                    .EnsureEndsWith("Attribute")
                                                                    .Equals(typeof(T).Name));
            return symbol;
        }

        public static string? GetCanExecuteMethodName(this IMethodSymbol method)
        {
            var attribute = method.GetCommandAttributeReference();

            if (attribute.Value.Value is not string canExecuteMethodName)
                return default;

            return ((INamedTypeSymbol)method.ContainingSymbol.OriginalDefinition)
                                                             .GetMembers()
                                                             .OfType<IMethodSymbol>()
                                                             .FirstOrDefault(x => x.Name == canExecuteMethodName).Name;
        }

        public static AvaloniaPropertyParts ParseAvaloniaProperty(this IFieldSymbol property)
        {
            return default!;
        }


        public static string? GetNamespaceRecursively(this INamespaceSymbol? symbol)
        {
            if (symbol?.ContainingNamespace == null)
            {
                return symbol?.Name;
            }

            return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
        }

        public static ReactiveCommandParts ParseReactiveCommand(this IMethodSymbol method)
        {
            IParameterSymbol? firstParameter = method.Parameters.FirstOrDefault();
            string tParam = firstParameter is ITypeSymbol typeSymbol
                ? $"{GetNamespaceRecursively(typeSymbol.ContainingNamespace)}.{typeSymbol.Name}" 
                : firstParameter is not null
                    ? $"{GetNamespaceRecursively(firstParameter.Type.ContainingNamespace)}.{firstParameter.Type.Name}"
                    : BaseReactiveCommandDeclaration.UnitTypeName;

            string commandName = $"{method.Name}Command";
            bool isTask = TryGetTaskResult(method, out string tResult);

            ReactiveCommandParts defaultCommand = new(tParam, tResult, commandName)
            {
                IsTask = isTask,
                MethodName = method.Name,
                CanExecute = method.GetCanExecuteMethodName()
            };

            return defaultCommand.SetPublicDeclaration();
        }

        public static bool TryGetTaskResult(this IMethodSymbol method, out string tResult)
        {
            INamedTypeSymbol returnTypeSymbol = (INamedTypeSymbol)method.ReturnType;
            bool isTask = returnTypeSymbol.Name == typeof(Task).Name;

            if (isTask)
            {
                tResult = returnTypeSymbol.TypeArguments.FirstOrDefault() is ITypeSymbol typeSymbol
                    ? $"{GetNamespaceRecursively(typeSymbol.ContainingNamespace)}.{typeSymbol.Name}"
                    : BaseReactiveCommandDeclaration.UnitTypeName;
            }
            else
            {
                tResult = method.ReturnsVoid
                    ? BaseReactiveCommandDeclaration.UnitTypeName
                    : $"{GetNamespaceRecursively(returnTypeSymbol.ContainingNamespace)}.{returnTypeSymbol.Name}";
            }

            return isTask;
        }

        private static KeyValuePair<string, TypedConstant> GetCommandAttributeReference(this IMethodSymbol method)
        {
            return method.GetAttributes()[0].NamedArguments.FirstOrDefault(x => x.Key == nameof(ReactiveCommandParts.CanExecute));
        }
    }
}