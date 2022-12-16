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

    /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => weakEventManager.AddEventHandler(value);
        remove => weakEventManager.RemoveEventHandler(value);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="e">The input <see cref="PropertyChangedEventArgs"/> instance.</param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="e"/> is <see langword="null"/>.</exception>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        weakEventManager.HandleEvent(this, e, nameof(INotifyPropertyChanged.PropertyChanged));
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="PropertyChanging"/> event, updates the property with the new
    /// value, then raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised
    /// if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetProperty<T>([NotNullIfNotNull("newValue")] ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        // We duplicate the code here instead of calling the overload because we can't
        // guarantee that the invoked SetProperty<T> will be inlined, and we need the JIT
        // to be able to see the full EqualityComparer<T>.Default.Equals call, so that
        // it'll use the intrinsics version of it and just replace the whole invocation
        // with a direct comparison when possible (eg. for primitive numeric types).
        // This is the fastest SetProperty<T> overload so we particularly care about
        // the codegen quality here, and the code is small and simple enough so that
        // duplicating it still doesn't make the whole class harder to maintain.
        if (EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }

        field = newValue;

        OnPropertyChanged(propertyName);

        return true;
    }
}