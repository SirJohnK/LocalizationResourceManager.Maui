namespace LocalizationResourceManager.Maui;

/// <summary>
/// Interface used to specify the resource manager to use for a specific page.
/// Used by the SpecificResourceManager source generator.
/// </summary>
public interface ISpecificResourceManager
{
    /// <summary>
    /// Name of the specific registered resource manager.
    /// </summary>
    public string ResourceManager { get; }
}