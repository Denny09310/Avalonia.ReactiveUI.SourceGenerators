using Avalonia.ReactiveUI.SourceGenerators.Generation.Models;
using Avalonia.ReactiveUI.SourceGenerators.Models.Base;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions
{
    internal static class SymbolExtensions
    {
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

        public static ReactiveCommandParts ParseCommand(this IMethodSymbol method)
        {
            string tParam = method.Parameters.Any() ? method.Parameters[0].Type.Name : BaseReactiveCommandDeclaration.UnitTypeName;
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
                tResult = returnTypeSymbol.TypeArguments.Any()
                    ? returnTypeSymbol.TypeArguments[0].Name
                    : BaseReactiveCommandDeclaration.UnitTypeName;
            }
            else
            {
                tResult = method.ReturnsVoid
                    ? BaseReactiveCommandDeclaration.UnitTypeName
                    : returnTypeSymbol.Name;
            }

            return isTask;
        }

        private static KeyValuePair<string, TypedConstant> GetCommandAttributeReference(this IMethodSymbol method)
        {
            return method.GetAttributes()[0].NamedArguments.FirstOrDefault(x => x.Key == nameof(ReactiveCommandParts.CanExecute));
        }
    }
}