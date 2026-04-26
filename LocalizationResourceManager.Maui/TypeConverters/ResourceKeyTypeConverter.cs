using System.ComponentModel;
using System.Reflection;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// TypeConverter that provides resource key suggestions for the Visual Studio XAML designer.
/// Scans all loaded assemblies for source-generated *Keys classes and returns their string constants.
/// </summary>
public class ResourceKeyTypeConverter : TypeConverter
{
    /// <inheritdoc/>
    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

    /// <inheritdoc/>
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

    /// <inheritdoc/>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        var keys = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); } catch { return []; } })
            .Where(t => t.Name.EndsWith("Keys") && t.IsAbstract && t.IsSealed)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => (string?)f.GetRawConstantValue()))
            .OfType<string>()
            .Order()
            .ToList();

        return new StandardValuesCollection(keys);
    }
}
