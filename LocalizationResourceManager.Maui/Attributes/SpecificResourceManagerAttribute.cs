namespace LocalizationResourceManager.Maui;

[AttributeUsage(AttributeTargets.Class)]
public class SpecificResourceManagerAttribute(string ResourceManager) : Attribute
{
    public string ResourceManager { get; } = ResourceManager;
}