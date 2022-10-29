using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Avalonia.ReactiveUI.SourceGenerators.Extensions;

internal static class SourceGeneratorExtensions
{
    private const string _templatesNamespace = "Templates";

    public static string GetEmbededResource<T>(this T sourceGenerator, string path)
        where T: class
    {
        var sourceAssembly = sourceGenerator.GetType().Assembly;
        var sourceAssemblyName = sourceAssembly.GetName().Name;
        var manifestResourceName = string.Join(".", sourceAssemblyName, _templatesNamespace, path);

        using var stream = sourceAssembly.GetManifestResourceStream(manifestResourceName);

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }
}
