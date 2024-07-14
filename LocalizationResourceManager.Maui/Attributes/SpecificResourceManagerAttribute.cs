namespace LocalizationResourceManager.Maui;

/// <summary>
/// Attribute used to specify the resource manager to use for a specific page.
/// Used by the SpecificResourceManager source generator.
/// </summary>
/// <param name="ResourceManager">Name of the specific registered resource manager.</param>
[AttributeUsage(AttributeTargets.Class)]
public class SpecificResourceManagerAttribute(string ResourceManager) : Attribute
{
    /// <summary>
    /// Name of the specific registered resource manager.
    /// </summary>
    public string ResourceManager { get; } = ResourceManager;
}