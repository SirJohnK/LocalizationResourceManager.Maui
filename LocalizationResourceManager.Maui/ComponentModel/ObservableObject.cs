using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LocalizationResourceManager.Maui.ComponentModel;

/// <summary>
/// A base class for objects of which the properties must be observable.
/// </summary>
/// <remarks>
/// Uses WeakEventManager to avoid memory leaks!
/// </remarks>
public abstract class ObservableObject : INotifyPropertyChanged
{
    private readonly WeakEventManager weakEventManager = new WeakEventManager();

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    /// <summary>
    /// Trigger the PropertyChanged event with the provided event arguments.
    /// </summary>
    /// <param name="e">The Property changed event information.</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        weakEventManager.HandleEvent(this, e, nameof(INotifyPropertyChanged.PropertyChanged));
    }

    /// <summary>
    /// Trigger the PropertyChanged event with the provided property name.
    /// </summary>
    /// <param name="propertyName">Name of the changed property.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the property and notifies listeners if the value has changed.
    /// </summary>
    /// <typeparam name="T">Property generic type.</typeparam>
    /// <param name="field">Property field to be set.</param>
    /// <param name="newValue">New value to set property field with.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected bool SetProperty<T>([NotNullIfNotNull("newValue")] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue))
            return false;

        field = newValue;
        OnPropertyChanged(propertyName);

        return true;
    }
}