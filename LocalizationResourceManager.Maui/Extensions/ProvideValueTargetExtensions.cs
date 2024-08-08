using System.Reflection;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Extension methods for <see cref="IProvideValueTarget"/>.
/// </summary>
public static class ProvideValueTargetExtensions
{
    /// <summary>
    /// Get the root object from the <see cref="IProvideValueTarget"/>.
    /// </summary>
    /// <param name="provideValueTarget">The <see cref="IProvideValueTarget"/> to retrieve the root object from.</param>
    /// <returns>The root object of the provided value target.</returns>
    public static object? GetRootObject(this IProvideValueTarget provideValueTarget)
    {
        //Attemp to get the root object from the provided value target
        var pvtType = provideValueTarget?.GetType();
        var parentsInfo = pvtType?.GetProperty("Microsoft.Maui.Controls.Xaml.IProvideParentValues.ParentObjects", BindingFlags.NonPublic | BindingFlags.Instance);
        var parents = parentsInfo?.GetValue(provideValueTarget) as IEnumerable<object>;
        return parents?.LastOrDefault();
    }
}