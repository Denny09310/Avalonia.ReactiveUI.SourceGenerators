namespace Avalonia.ReactiveUI.SourceGenerators.Generation.Extensions;

internal static class StringExtensions
{
    public static string EnsureEndsWith(this string source, string suffix)
    {
        if (source.EndsWith(suffix))
        {
            return source;
        }

        return source + suffix;
    }

    public static string ToCamelCase(this string source)
    {
        return char.ToLowerInvariant(source[0]) + source.Substring(1);
    }

    public static string ToPascalCase(this string source)
    {
        return char.ToUpperInvariant(source[0]) + source.Substring(1);
    }
}