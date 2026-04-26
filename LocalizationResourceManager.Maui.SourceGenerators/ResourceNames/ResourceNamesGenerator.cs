using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace LocalizationResourceManager.Maui.SourceGenerators;

/// <summary>
/// Source Generator that scans the consuming project directory for main .resx files
/// and emits a {Name}Keys static class per file containing public const string members
/// for every resource key.
/// </summary>
[Generator(LanguageNames.CSharp)]
public class ResourceNamesGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor DuplicateIdentifierDiagnostic = new(
        "LRMSG001",
        "Duplicate sanitized resource key identifier",
        "Resource key '{0}' produces duplicate identifier '{1}' (already used by '{2}') and will be skipped in the generated enum",
        "SourceGenerator",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Initialize the Source Generator.
    /// </summary>
    /// <param name="context">Source generator context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var config = context.AnalyzerConfigOptionsProvider
            .Combine(context.CompilationProvider)
            .Select((pair, _) =>
            {
                pair.Left.GlobalOptions.TryGetValue("build_property.projectdir", out var projectDir);
                pair.Left.GlobalOptions.TryGetValue("build_property.rootnamespace", out var rootNamespace);
                return (ProjectDir: projectDir, RootNamespace: rootNamespace);
            });

        context.RegisterSourceOutput(config, GenerateResourceKeys);
    }

    private void GenerateResourceKeys(SourceProductionContext context, (string? ProjectDir, string? RootNamespace) config)
    {
        if (string.IsNullOrWhiteSpace(config.ProjectDir))
            return;

#pragma warning disable RS1035 // File I/O is intentional in this source generator
        var mainResxFiles = Directory
            .GetFiles(config.ProjectDir!, "*.resx", SearchOption.AllDirectories)
            .Where(f => !Path.GetFileNameWithoutExtension(f).Contains('.'));

        foreach (var filePath in mainResxFiles)
        {
            try
            {
                var fileContent = File.ReadAllText(filePath);
#pragma warning restore RS1035
                var keys = ParseResourceKeys(fileContent);
                var fileName = Path.GetFileNameWithoutExtension(filePath);

                // Existing: static class with string constants
                var source = GenerateKeysClassSource(fileName, config.RootNamespace, keys);
                context.AddSource($"{fileName}Keys.g.cs", source);

                // New: enum + typed markup extension (skip if no keys)
                if (keys.Length > 0)
                {
                    var uniqueKeys = GetUniqueKeys(context, keys);
                    var enumSource = GenerateEnumSource(fileName, config.RootNamespace, uniqueKeys);
                    context.AddSource($"{fileName}Key.g.cs", enumSource);

                    var extensionSource = GenerateTypedExtensionSource(fileName, config.RootNamespace, uniqueKeys);
                    context.AddSource($"Translate{fileName}Extension.g.cs", extensionSource);
                }
            }
            catch
            {
                // Skip files that cannot be read or parsed
            }
        }
    }

    private static string[] ParseResourceKeys(string fileContent)
    {
        var doc = XDocument.Parse(fileContent);
        return doc.Root?
            .Elements("data")
            .Where(e => e.Attribute("type") == null && e.Attribute("mimetype") == null)
            .Select(e => e.Attribute("name")?.Value)
            .Where(name => name != null)
            .Select(name => name!)
            .ToArray() ?? System.Array.Empty<string>();
    }

    /// <summary>
    /// Deduplicates keys by sanitized identifier, reporting diagnostics for duplicates.
    /// Returns list of (originalKey, sanitizedIdentifier) pairs.
    /// </summary>
    private static List<(string OriginalKey, string Identifier)> GetUniqueKeys(SourceProductionContext context, string[] keys)
    {
        var result = new List<(string, string)>();
        var seen = new Dictionary<string, string>(); // identifier -> first original key

        foreach (var key in keys)
        {
            var identifier = SanitizeIdentifier(key);
            if (seen.TryGetValue(identifier, out var existingKey))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DuplicateIdentifierDiagnostic, Location.None, key, identifier, existingKey));
            }
            else
            {
                seen[identifier] = key;
                result.Add((key, identifier));
            }
        }

        return result;
    }

    private static string GenerateKeysClassSource(string fileName, string? rootNamespace, string[] keys)
    {
        var className = $"{fileName}Keys";
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");

        var hasNamespace = !string.IsNullOrWhiteSpace(rootNamespace);
        if (hasNamespace)
        {
            sb.AppendLine($"namespace {rootNamespace}");
            sb.AppendLine("{");
        }

        var indent = hasNamespace ? "    " : "";
        sb.AppendLine($"{indent}public static partial class {className}");
        sb.AppendLine($"{indent}{{");

        foreach (var key in keys)
        {
            var identifier = SanitizeIdentifier(key);
            sb.AppendLine($"{indent}    public const string {identifier} = \"{key}\";");
        }

        sb.AppendLine($"{indent}}}");

        if (hasNamespace)
            sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateEnumSource(string fileName, string? rootNamespace, List<(string OriginalKey, string Identifier)> keys)
    {
        var enumName = $"{fileName}Key";
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");

        var hasNamespace = !string.IsNullOrWhiteSpace(rootNamespace);
        if (hasNamespace)
        {
            sb.AppendLine($"namespace {rootNamespace}");
            sb.AppendLine("{");
        }

        var indent = hasNamespace ? "    " : "";
        sb.AppendLine($"{indent}public enum {enumName}");
        sb.AppendLine($"{indent}{{");

        for (var i = 0; i < keys.Count; i++)
        {
            var comma = i < keys.Count - 1 ? "," : "";
            sb.AppendLine($"{indent}    {keys[i].Identifier}{comma}");
        }

        sb.AppendLine($"{indent}}}");

        if (hasNamespace)
            sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateTypedExtensionSource(string fileName, string? rootNamespace, List<(string OriginalKey, string Identifier)> keys)
    {
        var enumName = $"{fileName}Key";
        var className = $"Translate{fileName}Extension";
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");

        var hasNamespace = !string.IsNullOrWhiteSpace(rootNamespace);
        if (hasNamespace)
        {
            sb.AppendLine($"namespace {rootNamespace}");
            sb.AppendLine("{");
        }

        var indent = hasNamespace ? "    " : "";

        sb.AppendLine($"{indent}[global::Microsoft.Maui.Controls.ContentProperty(nameof(Key))]");
        sb.AppendLine($"{indent}[global::Microsoft.Maui.Controls.Xaml.RequireService([typeof(global::Microsoft.Maui.Controls.Xaml.IProvideValueTarget)])]");
        sb.AppendLine($"{indent}public class {className} : global::Microsoft.Maui.Controls.Xaml.IMarkupExtension<global::Microsoft.Maui.Controls.BindingBase>");
        sb.AppendLine($"{indent}{{");
        sb.AppendLine($"{indent}    public {enumName} Key {{ get; set; }}");
        sb.AppendLine($"{indent}    public string StringFormat {{ get; set; }}");
        sb.AppendLine($"{indent}    public global::Microsoft.Maui.Controls.IValueConverter Converter {{ get; set; }}");
        sb.AppendLine($"{indent}    public object ConverterParameter {{ get; set; }}");
        sb.AppendLine($"{indent}    public string ResourceManager {{ get; set; }}");
        sb.AppendLine();
        sb.AppendLine($"{indent}    object global::Microsoft.Maui.Controls.Xaml.IMarkupExtension.ProvideValue(System.IServiceProvider serviceProvider) => ProvideValue(serviceProvider);");
        sb.AppendLine();
        sb.AppendLine($"{indent}    public global::Microsoft.Maui.Controls.BindingBase ProvideValue(System.IServiceProvider serviceProvider)");
        sb.AppendLine($"{indent}    {{");
        sb.AppendLine($"{indent}        var ext = new global::LocalizationResourceManager.Maui.TranslateExtension");
        sb.AppendLine($"{indent}        {{");
        sb.AppendLine($"{indent}            Text = KeyToString(Key),");
        sb.AppendLine($"{indent}            StringFormat = StringFormat,");
        sb.AppendLine($"{indent}            Converter = Converter,");
        sb.AppendLine($"{indent}            ConverterParameter = ConverterParameter,");
        sb.AppendLine($"{indent}            ResourceManager = ResourceManager");
        sb.AppendLine($"{indent}        }};");
        sb.AppendLine($"{indent}        return ext.ProvideValue(serviceProvider);");
        sb.AppendLine($"{indent}    }}");
        sb.AppendLine();
        sb.AppendLine($"{indent}    private static string KeyToString({enumName} key) => key switch");
        sb.AppendLine($"{indent}    {{");

        // Only emit explicit switch arms for keys where sanitized name differs from original
        foreach (var (originalKey, identifier) in keys)
        {
            if (identifier != originalKey)
            {
                sb.AppendLine($"{indent}        {enumName}.{identifier} => \"{originalKey}\",");
            }
        }

        sb.AppendLine($"{indent}        _ => key.ToString()");
        sb.AppendLine($"{indent}    }};");

        sb.AppendLine($"{indent}}}");

        if (hasNamespace)
            sb.AppendLine("}");

        return sb.ToString();
    }

    private static string SanitizeIdentifier(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "_";

        var sb = new StringBuilder(name.Length);
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
                sb.Append(c);
            else
                sb.Append('_');
        }

        var result = sb.ToString();
        if (char.IsDigit(result[0]))
            result = "_" + result;

        return result;
    }
}
