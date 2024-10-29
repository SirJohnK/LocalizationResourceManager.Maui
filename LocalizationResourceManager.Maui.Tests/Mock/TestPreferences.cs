namespace LocalizationResourceManager.Maui.Tests.Mock;

internal class TestPreferences : IPreferences
{
    private const string SharedName = "Shared_Preferences";
    private readonly Dictionary<string, Dictionary<string, object>> preferences = new() { { SharedName, new() } };

    public void Clear(string? sharedName = null)
    {
        if (sharedName is null)
            //Clear Shared Preferences Dictionary
            preferences[SharedName].Clear();
        else
            //Clear Shared Name Dictionary
            preferences.Remove(sharedName);
    }

    public bool ContainsKey(string key, string? sharedName = null)
    {
        //Resolve Shared Name Dictionary
        sharedName ??= SharedName;

        //Return True if Key Exists in Shared Name Dictionary
        return preferences.ContainsKey(sharedName) && preferences[sharedName].ContainsKey(key);
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        //Resolve Shared Name Dictionary
        sharedName ??= SharedName;

        //Return Value from Shared Name Dictionary or Default Value
        return preferences.ContainsKey(sharedName) && preferences[sharedName].ContainsKey(key) ? (T)preferences[sharedName][key] : defaultValue;
    }

    public void Remove(string key, string? sharedName = null)
    {
        //Resolve Shared Name Dictionary
        sharedName ??= SharedName;

        //Remove Key from Shared Name Dictionary
        preferences[sharedName].Remove(key);
    }

    public void Set<T>(string key, T value, string? sharedName = null)
    {
        //Verify Parameters
        ArgumentNullException.ThrowIfNull(value);

        //Resolve Shared Name Dictionary
        sharedName ??= SharedName;

        //Check if Shared Name Dictionary Exists
        if (preferences.ContainsKey(sharedName))
            //Set Key in Shared Name Dictionary
            preferences[sharedName][key] = value;
        else
            //Add Shared Name Dictionary
            preferences[sharedName] = new() { { key, value } };
    }
}